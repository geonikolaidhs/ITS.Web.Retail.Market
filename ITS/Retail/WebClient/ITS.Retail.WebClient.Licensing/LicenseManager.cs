using ITS.Licensing.Client.Core;
using ITS.Licensing.Common;
using System;

namespace ITS.Retail.WebClient.Licensing
{
    public class LicenseManager
    {
        IClientKeyGenerator ClientKeyGenerator;
        ILicenseInfoStorageReader LicenseInfoStorageReader;
        ILicenseInfoStorageWriter LicenseInfoStorageWriter;
        ILicenseActivator LicenseActivator;
        ILicenseStatusResolver LicenseStatusResolver;
        IActivationKeyValidator ActivationKeyValidator;
        IActivationKeyInfoExtractor ActivationKeyInfoExtractor;

        public event EventHandler OnUpdateLicense;

        public LicenseManager(IClientKeyGenerator clientKeyGenerator, ILicenseInfoStorageReader licenseInfoStorageReader, ILicenseInfoStorageWriter licenseInfoStorageWriter,
                              ILicenseActivator licenseActivator, ILicenseStatusResolver licenseStatusResolver, IActivationKeyValidator activationKeyValidator,
                                IActivationKeyInfoExtractor activationKeyInfoExtractor)
        {
            this.ClientKeyGenerator = clientKeyGenerator;
            this.LicenseActivator = licenseActivator;
            this.LicenseInfoStorageWriter = licenseInfoStorageWriter;
            this.LicenseInfoStorageReader = licenseInfoStorageReader;
            this.LicenseStatusResolver = licenseStatusResolver;
            this.ActivationKeyValidator = activationKeyValidator;
            this.ActivationKeyInfoExtractor = activationKeyInfoExtractor;
        }

        public void RequestActivation(string machineName, string machineID, string serialNumber, Guid assemblyID, Version currentVersion)
        {
            byte[] clientKey = this.ClientKeyGenerator.GenerateClientKey(assemblyID, machineID, serialNumber);
            OperationResult<GetActivationKeyResult> activatorResult = this.LicenseActivator.GetActivationKey(machineName, machineID, serialNumber, clientKey, currentVersion);
            if (activatorResult.Failure)
            {
                throw new Exception("Activation Failed: " + activatorResult.Error);
            }

            OperationResult<ExtractActivationKeyInfoResult> extractorResult = ActivationKeyInfoExtractor.ExtractActivationKeyInfo(activatorResult.Value.ActivationKey, clientKey);
            if (extractorResult.Failure)
            {
                throw new Exception("Extracting info from activation key failed: " + extractorResult.Error);
            }

            LicenseInfo licenseInfo = new LicenseInfo();
            licenseInfo.ActivationKey = activatorResult.Value.ActivationKey;
            licenseInfo.LicenseType = activatorResult.Value.LicenseType;
            licenseInfo.SerialNumber = serialNumber;
            licenseInfo.MaxUsers = extractorResult.Value.MaxUsers;
            licenseInfo.MaxPeripheralsUsers = extractorResult.Value.MaxPeripheralUsers;
            licenseInfo.MaxTabletSmartPhoneUsers = extractorResult.Value.MaxTabletSmartphoneUsers;
            licenseInfo.StartDate = extractorResult.Value.LicenseStartDate;
            licenseInfo.EndDate = extractorResult.Value.LicenseEndDate;
            licenseInfo.GreyZoneDays = extractorResult.Value.GreyZoneDays;
            licenseInfo.DaysToAlertBeforeExpiration = extractorResult.Value.DaysToAlertBeforeExpiration;

            StoreLicenseInfo(licenseInfo);
        }

        public GetLicenseStatusResult GetLicenseStatus(int currentUsersCount, int currentPeripheralUsersCount, int currentTabletSmartPhoneUsersCount, string machineID, Guid assemblyID, Version currentVersion)
        {
            LicenseInfo licenseInfo = ReadStoredLicenseInfo();
            byte[] clientKey = ClientKeyGenerator.GenerateClientKey(assemblyID, machineID, licenseInfo.SerialNumber);
            if (ActivationKeyValidator.IsActivationKeyValid(licenseInfo.ActivationKey, clientKey) == false)
            {
                throw new Exception("Invalid Activation Key");
            }
            OperationResult<GetLicenseStatusResult> result = LicenseStatusResolver.GetLicenseStatus(currentUsersCount,
                                                                                                    currentPeripheralUsersCount,
                                                                                                    currentTabletSmartPhoneUsersCount,
                                                                                                    licenseInfo.ActivationKey,
                                                                                                    licenseInfo.SerialNumber,
                                                                                                    machineID,
                                                                                                    assemblyID,
                                                                                                    licenseInfo.LicenseType,
                                                                                                    clientKey,
                                                                                                    currentVersion
                                                                                                    );
            if (result.Failure)
            {
                throw new Exception("Get License Status error:" + result.Error);
            }

            if (result.Value.HasNewLicenseType || result.Value.HasNewActivationKey)
            {

                if (result.Value.HasNewLicenseType)
                {
                    licenseInfo.LicenseType = result.Value.NewLicenseType;
                }
                if (result.Value.HasNewActivationKey)
                {
                    OperationResult<ExtractActivationKeyInfoResult> extractorResult = ActivationKeyInfoExtractor.ExtractActivationKeyInfo(result.Value.NewActivationKey, clientKey);
                    if (extractorResult.Failure)
                    {
                        throw new Exception("Extracting info from activation key failed: " + extractorResult.Error);
                    }
                    licenseInfo.ActivationKey = result.Value.NewActivationKey;
                    licenseInfo.StartDate = extractorResult.Value.LicenseStartDate;
                    licenseInfo.EndDate = extractorResult.Value.LicenseEndDate;
                    licenseInfo.MaxUsers = extractorResult.Value.MaxUsers;
                    licenseInfo.MaxPeripheralsUsers = extractorResult.Value.MaxPeripheralUsers;
                    licenseInfo.MaxTabletSmartPhoneUsers = extractorResult.Value.MaxTabletSmartphoneUsers;
                    licenseInfo.GreyZoneDays = extractorResult.Value.GreyZoneDays;
                    licenseInfo.DaysToAlertBeforeExpiration = extractorResult.Value.DaysToAlertBeforeExpiration;
                }
                StoreLicenseInfo(licenseInfo);
            }
            else if (result.Value.LicenseStatus == ITS.Licensing.Enumerations.LicenseStatus.Invalid)
            {
                licenseInfo.StartDate = new DateTime(1601, 1, 2);
                licenseInfo.EndDate = new DateTime(1601, 1, 2);
                StoreLicenseInfo(licenseInfo);
            }

            return result.Value;
        }

        protected void StoreLicenseInfo(LicenseInfo licenseInfo)
        {

            try
            {
                LicenseInfo readLicenseInfo = ReadStoredLicenseInfo();
                if (licenseInfo.Equals(readLicenseInfo))
                {
                    return;
                }
            }
            catch (Exception exception)
            {

            }


            OperationResult writerResult = LicenseInfoStorageWriter.StoreLicenseInfo(licenseInfo);
            if (writerResult.Failure)
            {
                throw new Exception("Write License storage failed: " + writerResult.Error);
            }

            if (this.OnUpdateLicense != null)
            {
                this.OnUpdateLicense(this, new EventArgs());
            }
        }

        public LicenseInfo ReadStoredLicenseInfo()
        {
            OperationResult<LicenseInfo> result = this.LicenseInfoStorageReader.ReadLicenseInfo();
            if (result.Failure)
            {
                throw new Exception("Read License storage error:" + result.Error);
            }
            return result.Value;
        }
    }
}
