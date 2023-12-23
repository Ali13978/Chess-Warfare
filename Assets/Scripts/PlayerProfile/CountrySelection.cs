using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;

public class CountrySelection : MonoBehaviour
{
    public static CountrySelection Instance;

    [SerializeField]
    private Sprite []CountryImages;


    private void Awake()
    {
        Instance = this;
        DontDestroyOnLoad(this.gameObject);
    }

    public List<string> GetCountryCodeList()
    {
        List<string> countrylist = new List<string>();

        Dictionary<string, string> countryCodesMapping = new Dictionary<string, string>() {
           { "Afghanistan","AF" },    // Afghanistan
           { "Albania", "AL" },    // Albania
           { "U.A.E.", "AE" },    // U.A.E.
           { "Argentina", "AR" },    // Argentina
           { "Armenia", "AM" },    // Armenia
           { "Australia", "AU" },    // Australia
           { "Austria", "AT" },    // Austria
           { "Azerbaijan", "AZ" },    // Azerbaijan
           { "Belgium", "BE" },    // Belgium
           { "Bangladesh", "BD" },    // Bangladesh
           { "Bulgaria", "BG" },    // Bulgaria
           { "Bahrain", "BH" },    // Bahrain
           { "Bosnia", "BA" },    // Bosnia and Herzegovina
           { "Belarus", "BY" },    // Belarus
           { "Belize", "BZ" },    // Belize
           { "Bolivia", "BO" },    // Bolivia
           { "Brazil", "BR" },    // Brazil
           { "Brunei Darussalam", "BN" },    // Brunei Darussalam
           { "Canada", "CA" },    // Canada
           { "Switzerland", "CH" },    // Switzerland
           { "Chile", "CL" },    // Chile
           { "China", "CN" },    // People's Republic of China
           { "Colombia", "CO" },    // Colombia
           { "Costa Rica", "CR" },    // Costa Rica
           { "Czech Republic", "CZ" },    // Czech Republic
           { "Germany", "DE" },    // Germany
           { "Denmark", "DK" },    // Denmark
           { "Dominican Republic", "DO" },    // Dominican Republic
           { "Algeria", "DZ" },    // Algeria
           { "Ecuador", "EC" },    // Ecuador
           { "Egypt", "EG" },    // Egypt
           { "Spain", "ES" },    // Spain
           { "Estonia", "EE" },    // Estonia
           { "Ethiopia", "ET" },    // Ethiopia
           { "Finland", "FI" },    // Finland
           { "France", "FR" },    // France
           { "Faroe Islands", "FO" },    // Faroe Islands
           { "United Kingdom", "GB" },    // United Kingdom
           { "Georgia", "GE" },    // Georgia
           { "Greece", "GR" },    // Greece
           { "Greenland", "GL" },    // Greenland
           { "Guatemala", "GT" },    // Guatemala
           { "Hong Kong S.A.R.", "HK" },    // Hong Kong S.A.R.
           { "Honduras", "HN" },    // Honduras
           { "Croatia", "HR" },    // Croatia
           { "Hungary", "HU" },    // Hungary
           { "Indonesia", "ID" },    // Indonesia
           { "India", "IN" },    // India
           { "Ireland", "IE" },    // Ireland
           { "Iran", "IR" },    // Iran
           { "Iraq", "IQ" },    // Iraq
           { "Iceland", "IS" },    // Iceland
           { "Israel", "IL" },    // Israel
           { "Italy", "IT" },    // Italy
           { "Jamaica", "JM" },    // Jamaica
           { "Jordan", "JO" },    // Jordan
           { "Japan", "JP" },    // Japan
           { "Kazakhstan", "KZ" },    // Kazakhstan
           { "Kenya", "KE" },    // Kenya
           { "Kyrgyzstan", "KG" },    // Kyrgyzstan
           { "Cambodia", "KH" },    // Cambodia
           { "Korea", "KR" },    // Korea
           { "Kuwait", "KW" },    // Kuwait
           { "Lao P.D.R.", "LA" },    // Lao P.D.R.
           { "Lebanon", "LB" },    // Lebanon
           { "Libya", "LY" },    // Libya
           { "Liechtenstein", "LI" },    // Liechtenstein
           { "Sri Lanka", "LK" },    // Sri Lanka
           { "Lithuania", "LT" },    // Lithuania
           { "Luxembourg", "LU" },    // Luxembourg
           { "Latvia", "LV" },    // Latvia
           { "Macao S.A.R", "MO" },    // Macao S.A.R.
           { "Morocco", "MA" },    // Morocco
           { "Monaco", "MC" },    // Principality of Monaco
           { "Maldives", "MV" },    // Maldives
           { "Mexico", "MX" },    // Mexico
           { "Macedonia", "MK" },    // Macedonia (FYROM)
           { "Malta", "MT" },    // Malta
           { "Montenegro", "ME" },    // Montenegro
           { "Mongolia", "MN" },    // Mongolia
           { "Malaysia", "MY" },    // Malaysia
           { "Nigeria", "NG" },    // Nigeria
           { "Nicaragua", "NI" },    // Nicaragua
           { "Netherlands", "NL" },    // Netherlands
           { "Norway", "NO" },    // Norway
           { "Nepal", "NP" },    // Nepal
           { "New Zealand", "NZ" },    // New Zealand
           { "Oman", "OM" },    // Oman
           { "Pakistan", "PK" },    // Islamic Republic of Pakistan
           { "Panama", "PA" },    // Panama
           { "Peru", "PE" },    // Peru
           { "Philippines", "PH" },    // Republic of the Philippines
           { "Poland", "PL" },    // Poland
           { "Puerto Rico", "PR" },    // Puerto Rico
           { "Portugal", "PT" },    // Portugal
           { "Paraguay", "PY" },    // Paraguay
           { "Qatar", "QA" },    // Qatar
           { "Romania", "RO" },    // Romania
           { "Russia", "RU" },    // Russia
           { "Rwanda", "RW" },    // Rwanda
           { "Saudi Arabia", "SA" },    // Saudi Arabia
           { "Serbia and Montenegro", "CS" },    // Serbia and Montenegro (Former)
           { "Senegal", "SN" },    // Senegal
           { "Singapore", "SG" },    // Singapore
           { "El Salvador", "SV" },    // El Salvador
           { "Serbia", "RS" },    // Serbia
           { "Slovakia", "SK" },    // Slovakia
           { "Slovenia", "SI" },    // Slovenia
           { "Sweden", "SE" },    // Sweden
           { "Syria", "SY" },    // Syria
           { "Tajikistan", "TJ" },    // Tajikistan
           { "Thailand", "TH" },    // Thailand
           { "Turkmenistan", "TM" },    // Turkmenistan
           { "Trinidad and Tobago", "TT" },    // Trinidad and Tobago
           { "Tunisia", "TN" },    // Tunisia
           { "Turkey", "TR" },    // Turkey
           { "Taiwan", "TW" },    // Taiwan
           { "Ukraine", "UA" },    // Ukraine
           { "Uruguay", "UY" },    // Uruguay
           { "United States", "US" },    // United States
           { "Uzbekistan", "UZ" },    // Uzbekistan
           { "Venezuela", "VE" },    // Bolivarian Republic of Venezuela
           { "Vietnam", "VN" },    // Vietnam
           { "Yemen", "YE" },    // Yemen
           { "South Africa", "ZA" },    // South Africa
           { "Zimbabwe", "ZW" },    // Zimbabwe
        };
        Dictionary<string, string> countryCodesMappingOriginal = new Dictionary<string, string>();

        foreach(KeyValuePair<string, string> entry in countryCodesMapping)
        {
            countryCodesMappingOriginal.Add(entry.Value, entry.Key);
        }

        //foreach (KeyValuePair<string, string> entry in countryCodesMappingOriginal)
        //{
        //    Debug.Log("key " + entry.Key + " value " + entry.Value);
        //}

        foreach (Sprite sprite in CountryImages)
        {
            if(countryCodesMappingOriginal.ContainsKey(sprite.name.ToUpper()))
            {
                countrylist.Add(countryCodesMappingOriginal[sprite.name.ToUpper()]);
            }
            else
            {
                Debug.Log("does not exitst " + sprite.name.ToUpper());
            }
           
        }

        return countrylist;
    }

    public string GetCountryCode(string key)
    {
        Dictionary<string, string> countryCodesMapping = new Dictionary<string, string>() {
           { "Afghanistan","AF" },    // Afghanistan
           { "Albania", "AL" },    // Albania
           { "U.A.E.", "AE" },    // U.A.E.
           { "Argentina", "AR" },    // Argentina
           { "Armenia", "AM" },    // Armenia
           { "Australia", "AU" },    // Australia
           { "Austria", "AT" },    // Austria
           { "Azerbaijan", "AZ" },    // Azerbaijan
           { "Belgium", "BE" },    // Belgium
           { "Bangladesh", "BD" },    // Bangladesh
           { "Bulgaria", "BG" },    // Bulgaria
           { "Bahrain", "BH" },    // Bahrain
           { "Bosnia", "BA" },    // Bosnia and Herzegovina
           { "Belarus", "BY" },    // Belarus
           { "Belize", "BZ" },    // Belize
           { "Bolivia", "BO" },    // Bolivia
           { "Brazil", "BR" },    // Brazil
           { "Brunei Darussalam", "BN" },    // Brunei Darussalam
           { "Canada", "CA" },    // Canada
           { "Switzerland", "CH" },    // Switzerland
           { "Chile", "CL" },    // Chile
           { "China", "CN" },    // People's Republic of China
           { "Colombia", "CO" },    // Colombia
           { "Costa Rica", "CR" },    // Costa Rica
           { "Czech Republic", "CZ" },    // Czech Republic
           { "Germany", "DE" },    // Germany
           { "Denmark", "DK" },    // Denmark
           { "Dominican Republic", "DO" },    // Dominican Republic
           { "Algeria", "DZ" },    // Algeria
           { "Ecuador", "EC" },    // Ecuador
           { "Egypt", "EG" },    // Egypt
           { "Spain", "ES" },    // Spain
           { "Estonia", "EE" },    // Estonia
           { "Ethiopia", "ET" },    // Ethiopia
           { "Finland", "FI" },    // Finland
           { "France", "FR" },    // France
           { "Faroe Islands", "FO" },    // Faroe Islands
           { "United Kingdom", "GB" },    // United Kingdom
           { "Georgia", "GE" },    // Georgia
           { "Greece", "GR" },    // Greece
           { "Greenland", "GL" },    // Greenland
           { "Guatemala", "GT" },    // Guatemala
           { "Hong Kong S.A.R.", "HK" },    // Hong Kong S.A.R.
           { "Honduras", "HN" },    // Honduras
           { "Croatia", "HR" },    // Croatia
           { "Hungary", "HU" },    // Hungary
           { "Indonesia", "ID" },    // Indonesia
           { "India", "IN" },    // India
           { "Ireland", "IE" },    // Ireland
           { "Iran", "IR" },    // Iran
           { "Iraq", "IQ" },    // Iraq
           { "Iceland", "IS" },    // Iceland
           { "Israel", "IL" },    // Israel
           { "Italy", "IT" },    // Italy
           { "Jamaica", "JM" },    // Jamaica
           { "Jordan", "JO" },    // Jordan
           { "Japan", "JP" },    // Japan
           { "Kazakhstan", "KZ" },    // Kazakhstan
           { "Kenya", "KE" },    // Kenya
           { "Kyrgyzstan", "KG" },    // Kyrgyzstan
           { "Cambodia", "KH" },    // Cambodia
           { "Korea", "KR" },    // Korea
           { "Kuwait", "KW" },    // Kuwait
           { "Lao P.D.R.", "LA" },    // Lao P.D.R.
           { "Lebanon", "LB" },    // Lebanon
           { "Libya", "LY" },    // Libya
           { "Liechtenstein", "LI" },    // Liechtenstein
           { "Sri Lanka", "LK" },    // Sri Lanka
           { "Lithuania", "LT" },    // Lithuania
           { "Luxembourg", "LU" },    // Luxembourg
           { "Latvia", "LV" },    // Latvia
           { "Macao S.A.R", "MO" },    // Macao S.A.R.
           { "Morocco", "MA" },    // Morocco
           { "Monaco", "MC" },    // Principality of Monaco
           { "Maldives", "MV" },    // Maldives
           { "Mexico", "MX" },    // Mexico
           { "Macedonia", "MK" },    // Macedonia (FYROM)
           { "Malta", "MT" },    // Malta
           { "Montenegro", "ME" },    // Montenegro
           { "Mongolia", "MN" },    // Mongolia
           { "Malaysia", "MY" },    // Malaysia
           { "Nigeria", "NG" },    // Nigeria
           { "Nicaragua", "NI" },    // Nicaragua
           { "Netherlands", "NL" },    // Netherlands
           { "Norway", "NO" },    // Norway
           { "Nepal", "NP" },    // Nepal
           { "New Zealand", "NZ" },    // New Zealand
           { "Oman", "OM" },    // Oman
           { "Pakistan", "PK" },    // Islamic Republic of Pakistan
           { "Panama", "PA" },    // Panama
           { "Peru", "PE" },    // Peru
           { "Philippines", "PH" },    // Republic of the Philippines
           { "Poland", "PL" },    // Poland
           { "Puerto Rico", "PR" },    // Puerto Rico
           { "Portugal", "PT" },    // Portugal
           { "Paraguay", "PY" },    // Paraguay
           { "Qatar", "QA" },    // Qatar
           { "Romania", "RO" },    // Romania
           { "Russia", "RU" },    // Russia
           { "Rwanda", "RW" },    // Rwanda
           { "Saudi Arabia", "SA" },    // Saudi Arabia
           { "Serbia and Montenegro", "CS" },    // Serbia and Montenegro (Former)
           { "Senegal", "SN" },    // Senegal
           { "Singapore", "SG" },    // Singapore
           { "El Salvador", "SV" },    // El Salvador
           { "Serbia", "RS" },    // Serbia
           { "Slovakia", "SK" },    // Slovakia
           { "Slovenia", "SI" },    // Slovenia
           { "Sweden", "SE" },    // Sweden
           { "Syria", "SY" },    // Syria
           { "Tajikistan", "TJ" },    // Tajikistan
           { "Thailand", "TH" },    // Thailand
           { "Turkmenistan", "TM" },    // Turkmenistan
           { "Trinidad and Tobago", "TT" },    // Trinidad and Tobago
           { "Tunisia", "TN" },    // Tunisia
           { "Turkey", "TR" },    // Turkey
           { "Taiwan", "TW" },    // Taiwan
           { "Ukraine", "UA" },    // Ukraine
           { "Uruguay", "UY" },    // Uruguay
           { "United States", "US" },    // United States
           { "Uzbekistan", "UZ" },    // Uzbekistan
           { "Venezuela", "VE" },    // Bolivarian Republic of Venezuela
           { "Vietnam", "VN" },    // Vietnam
           { "Yemen", "YE" },    // Yemen
           { "South Africa", "ZA" },    // South Africa
           { "Zimbabwe", "ZW" },    // Zimbabwe
        };

        return countryCodesMapping[key];
    }

    public Sprite[] GetCountryImages()
    {
        return CountryImages;
    }
}
