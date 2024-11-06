using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;
using System.Linq;

namespace ITS.MobileAtStore.Common
{
    public class DecodingPattern : IXmlSubitems
    {
        public int Id { get; set; }
        public string Prefix { get; set; }
        public int StartIndex { get; set; }
        public int Length { get; set; }
        public string DecodingRule { get; set; }
        public PaddingSettings PaddingSettings { get; set; }
        public int PaddingSettingsLength
        {
            get
            {
                return this.PaddingSettings.Length;
            }
            set
            {
                this.PaddingSettings.Length = value;
            }
        }

        public char PaddingSettingsCharacter
        {
            get
            {
                return this.PaddingSettings.Character;
            }
            set
            {
                this.PaddingSettings.Character = value;
            }
        }

        public PaddingMode PaddingSettingsMode
        {
            get
            {
                return this.PaddingSettings.Mode;
            }
            set
            {
                this.PaddingSettings.Mode = value;
            }
        }

        public DecodingPattern()
        {
            PaddingSettings = new PaddingSettings();
        }

        [DataObjectMethod(DataObjectMethodType.Select)]
        public static List<DecodingPattern> GetDecodingPattern()
        {
            return Settings.DecodingSettings.DecodingPrefixes;
        }

        [DataObjectMethod(DataObjectMethodType.Insert)]
        public static int InsertDecodingPattern(string Prefix, int StartIndex, int Length, string DecodingRule, char Character,
            int PaddingLength, PaddingMode Mode, object PaddingSettingsMode,
            int PaddingSettingsLength, char PaddingSettingsCharacter
            )
        {
            PaddingSettings pad = new PaddingSettings() { Character = Character, Length = PaddingLength, Mode = Mode };
            int newId = Settings.DecodingSettings.DecodingPrefixes.Count == 0 ? 1 : Settings.DecodingSettings.DecodingPrefixes.Max(x => x.Id) + 1;
            DecodingPattern pattern = new DecodingPattern()
            {
                Id = newId,
                Length = Length,
                DecodingRule = DecodingRule,
                Prefix = Prefix,
                StartIndex = StartIndex,
                PaddingSettings = pad
            };
            Settings.DecodingSettings.DecodingPrefixes.Add(pattern);
            return 1;
        }

        [DataObjectMethod(DataObjectMethodType.Delete)]
        public static int DeleteDecodingPattern(int Id)
        {

            DecodingPattern decodingPattern = Settings.DecodingSettings.DecodingPrefixes.Where(x => x.Id == Id).FirstOrDefault();
            if (decodingPattern == null)
            {
                return 0;
            }
            Settings.DecodingSettings.DecodingPrefixes.Remove(decodingPattern);
            return 1;
        }

        [DataObjectMethod(DataObjectMethodType.Update)]
        public static int UpdateDecodingPattern(
            string Prefix, int Length, int StartIndex, string DecodingRule,
            PaddingMode PaddingSettingsMode, int PaddingSettingsLength, char PaddingSettingsCharacter, int Id)
        //(int Id, int Length, string DecodingRule,string Prefix,int StartIndex, PaddingSettings PaddingSettings)
        {
            DecodingPattern decodingPattern = Settings.DecodingSettings.DecodingPrefixes.Where(decodingPat => decodingPat.Id == Id).FirstOrDefault();
            if (decodingPattern == null)
            {
                return 0;
            }
            decodingPattern.Length = Length;
            decodingPattern.DecodingRule = DecodingRule;
            decodingPattern.Prefix = Prefix;
            decodingPattern.StartIndex = StartIndex;
            PaddingSettings paddingSettings = new PaddingSettings();
            paddingSettings.Character = PaddingSettingsCharacter;
            paddingSettings.Length = PaddingSettingsLength;
            paddingSettings.Mode = PaddingSettingsMode;
            decodingPattern.PaddingSettings = paddingSettings;
            return 1;
        }

    }
}
