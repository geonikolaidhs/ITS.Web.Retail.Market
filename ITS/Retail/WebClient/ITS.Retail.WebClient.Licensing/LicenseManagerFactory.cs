using ITS.Licensing.Client.Core;
using ITS.Licensing.Client.Core.Implementations;
using ITS.Licensing.Client.Services.Proxies.LicensingService;
using ITS.Licensing.Common;
using ITS.Licensing.Common.Implementations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ITS.Retail.WebClient.Licensing
{
    public static class LicenseManagerFactory
    {
        public static LicenseManager CreateLicenseManager(string licenseFilePath, ILicensingService service, ILogger logger)
        {
            IClientKeyGenerator clientKeyGenerator = new ClientKeyGenerator();
            IFileStreamProvider fileStreamProvider = new FileStreamProvider(licenseFilePath);
            LicenseInfoFileStorage licenseInfoFileStorage = new LicenseInfoFileStorage(fileStreamProvider);
            ILicenseActivator licenseActivator = new LoggingLicenseActivator(new LicenseOnlineActivator(service), logger);
            ICRCCalculator crcCalculator = new CRCCalculator();
            IDateTimeProvider dateTimeProvider = new DateTimeProvider();
            ILicenseStatusResolver licenseStatusResolver = new LoggingLicenseStatusResolver(
                                                                                    new LicenseOnlineStatusResolver(
                                                                                        new LoggingLicenseStatusResolver(
                                                                                                    new LicenseOfflineStatusResolver(null,
                                                                                                                                    new ActivationKeyInfoExtractor(crcCalculator),
                                                                                                                                    dateTimeProvider
                                                                                                                                    ),
                                                                                                    logger
                                                                                                    )
                                                                                    , service, logger)
                                                                                , logger);
            IActivationKeyInfoExtractor activationKeyInfoExtractor = new ActivationKeyInfoExtractor(crcCalculator);
            IActivationKeyValidator activationKeyValidator = new ActivationKeyValidator(activationKeyInfoExtractor);

            return new LicenseManager(clientKeyGenerator, licenseInfoFileStorage, licenseInfoFileStorage, licenseActivator, licenseStatusResolver, activationKeyValidator, activationKeyInfoExtractor);
        }
    }
}
