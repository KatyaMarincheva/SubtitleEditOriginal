﻿namespace Nikse.SubtitleEdit.Logic.ContainerFormats.Mp4.Boxes
{
    using System.Globalization;
    using System.IO;

    public class Mdhd : Box
    {
        public readonly ulong CreationTime;

        public readonly ulong Duration;

        public readonly string Iso639ThreeLetterCode;

        public readonly ulong ModificationTime;

        public readonly int Quality;

        public readonly uint TimeScale;

        public Mdhd(FileStream fs, ulong size)
        {
            this.Buffer = new byte[size - 4];
            fs.Read(this.Buffer, 0, this.Buffer.Length);
            int languageIndex = 20;
            int version = this.Buffer[0];
            if (version == 0)
            {
                this.CreationTime = this.GetUInt(4);
                this.ModificationTime = this.GetUInt(8);
                this.TimeScale = this.GetUInt(12);
                this.Duration = this.GetUInt(16);
                this.Quality = this.GetWord(22);
            }
            else
            {
                this.CreationTime = this.GetUInt64(4);
                this.ModificationTime = this.GetUInt64(12);
                this.TimeScale = this.GetUInt(16);
                this.Duration = this.GetUInt64(20);
                languageIndex = 24;
                this.Quality = this.GetWord(26);
            }

            // language code = skip first byte, 5 bytes + 5 bytes + 5 bytes (add 0x60 to get ascii value)
            int languageByte = ((this.Buffer[languageIndex] << 1) >> 3) + 0x60;
            int languageByte2 = ((this.Buffer[languageIndex] & 0x3) << 3) + (this.Buffer[languageIndex + 1] >> 5) + 0x60;
            int languageByte3 = (this.Buffer[languageIndex + 1] & 0x1f) + 0x60;
            char x = (char)languageByte;
            char x2 = (char)languageByte2;
            char x3 = (char)languageByte3;
            this.Iso639ThreeLetterCode = x.ToString(CultureInfo.InvariantCulture) + x2.ToString(CultureInfo.InvariantCulture) + x3.ToString(CultureInfo.InvariantCulture);
        }

        public string LanguageString
        {
            get
            {
                switch (this.Iso639ThreeLetterCode)
                {
                    case "abk":
                        return "Abkhazian";
                    case "ace":
                        return "Achinese";
                    case "ach":
                        return "Acoli";
                    case "ada":
                        return "Adangme";
                    case "aar":
                        return "Afar";
                    case "afh":
                        return "Afrihili";
                    case "afr":
                        return "Afrikaans";
                    case "afa":
                        return "Afro-Asiatic (Other)";
                    case "aka":
                        return "Akan";
                    case "akk":
                        return "Akkadian";
                    case "alb":
                        return "Albanian";
                    case "sqi":
                        return "Albanian";
                    case "ale":
                        return "Aleut";
                    case "alg":
                        return "Algonquian languages";
                    case "tut":
                        return "Altaic (Other)";
                    case "amh":
                        return "Amharic";
                    case "apa":
                        return "Apache languages";
                    case "ara":
                        return "Arabic";
                    case "arc":
                        return "Aramaic";
                    case "arg":
                        return "Arabic";
                    case "arp":
                        return "Arapaho";
                    case "arn":
                        return "Araucanian";
                    case "arw":
                        return "Arawak";
                    case "arm":
                        return "Armenian";
                    case "hye":
                        return "Armenian";
                    case "art":
                        return "Artificial (Other)";
                    case "asm":
                        return "Assamese";
                    case "ava":
                        return "Avaric";
                    case "ath":
                        return "Athapascan languages";
                    case "ave":
                        return "Avestan";
                    case "awa":
                        return "Awadhi";
                    case "aym":
                        return "Aymara";
                    case "aze":
                        return "Azerbaijani";
                    case "nah":
                        return "Aztec";
                    case "ban":
                        return "Balinese";
                    case "bat":
                        return "Baltic (Other)";
                    case "bal":
                        return "Baluchi";
                    case "bam":
                        return "Bambara";
                    case "bai":
                        return "Bamileke languages";
                    case "bad":
                        return "Banda";
                    case "bnt":
                        return "Bantu (Other)";
                    case "bas":
                        return "Basa";
                    case "bak":
                        return "Bashkir";
                    case "baq":
                        return "Basque";
                    case "eus":
                        return "Basque";
                    case "bej":
                        return "Beja";
                    case "bem":
                        return "Bemba";
                    case "ben":
                        return "Bengali";
                    case "ber":
                        return "Berber (Other)";
                    case "bho":
                        return "Bhojpuri";
                    case "bih":
                        return "Bihari";
                    case "bik":
                        return "Bikol";
                    case "bin":
                        return "Bini";
                    case "bis":
                        return "Bislama";
                    case "bra":
                        return "Braj";
                    case "bre":
                        return "Breton";
                    case "bug":
                        return "Buginese";
                    case "bul":
                        return "Bulgarian";
                    case "bua":
                        return "Buriat";
                    case "bur":
                        return "Burmese";
                    case "mya":
                        return "Burmese";
                    case "bel":
                        return "Byelorussian";
                    case "cad":
                        return "Caddo";
                    case "car":
                        return "Carib";
                    case "cat":
                        return "Catalan";
                    case "cau":
                        return "Caucasian (Other)";
                    case "ceb":
                        return "Cebuano";
                    case "cel":
                        return "Celtic (Other)";
                    case "cai":
                        return "Central American Indian (Other)";
                    case "chg":
                        return "Chagatai";
                    case "cha":
                        return "Chamorro";
                    case "che":
                        return "Chechen";
                    case "chr":
                        return "Cherokee";
                    case "chy":
                        return "Cheyenne";
                    case "chb":
                        return "Chibcha";
                    case "chi":
                        return "Chinese";
                    case "zho":
                        return "Chinese";
                    case "chn":
                        return "Chinook jargon";
                    case "cho":
                        return "Choctaw";
                    case "chu":
                        return "Church Slavic";
                    case "chv":
                        return "Chuvash";
                    case "cop":
                        return "Coptic";
                    case "cor":
                        return "Cornish";
                    case "cos":
                        return "Corsican";
                    case "cre":
                        return "Cree";
                    case "mus":
                        return "Creek";
                    case "crp":
                        return "Creoles and Pidgins (Other)";
                    case "cpe":
                        return "Creoles and Pidgins, English-based (Other)";
                    case "cpf":
                        return "Creoles and Pidgins, French-based (Other)";
                    case "cpp":
                        return "Creoles and Pidgins, Portuguese-based (Other)";
                    case "cus":
                        return "Cushitic (Other)";
                    case "   ":
                        return "Croatian";
                    case "ces":
                        return "Czech";
                    case "cze":
                        return "Czech";
                    case "dak":
                        return "Dakota";
                    case "dan":
                        return "Danish";
                    case "del":
                        return "Delaware";
                    case "din":
                        return "Dinka";
                    case "div":
                        return "Divehi";
                    case "doi":
                        return "Dogri";
                    case "dra":
                        return "Dravidian (Other)";
                    case "dua":
                        return "Duala";
                    case "dut":
                        return "Dutch";
                    case "nla":
                        return "Dutch";
                    case "dum":
                        return "Dutch, Middle (ca. 1050-1350)";
                    case "dyu":
                        return "Dyula";
                    case "dzo":
                        return "Dzongkha";
                    case "efi":
                        return "Efik";
                    case "egy":
                        return "Egyptian (Ancient)";
                    case "eka":
                        return "Ekajuk";
                    case "elx":
                        return "Elamite";
                    case "eng":
                        return "English";
                    case "enm":
                        return "English, Middle (ca. 1100-1500)";
                    case "ang":
                        return "English, Old (ca. 450-1100)";
                    case "esk":
                        return "Eskimo (Other)";
                    case "epo":
                        return "Esperanto";
                    case "est":
                        return "Estonian";
                    case "ewe":
                        return "Ewe";
                    case "ewo":
                        return "Ewondo";
                    case "fan":
                        return "Fang";
                    case "fat":
                        return "Fanti";
                    case "fao":
                        return "Faroese";
                    case "fij":
                        return "Fijian";
                    case "fin":
                        return "Finnish";
                    case "fiu":
                        return "Finno-Ugrian (Other)";
                    case "fon":
                        return "Fon";
                    case "fra":
                        return "French";
                    case "fre":
                        return "French";
                    case "frm":
                        return "French, Middle (ca. 1400-1600)";
                    case "fro":
                        return "French, Old (842- ca. 1400)";
                    case "fry":
                        return "Frisian";
                    case "ful":
                        return "Fulah";
                    case "gaa":
                        return "Ga";
                    case "gae":
                        return "Gaelic (Scots)";
                    case "gdh":
                        return "Gaelic (Scots)";
                    case "glg":
                        return "Gallegan";
                    case "lug":
                        return "Ganda";
                    case "gay":
                        return "Gayo";
                    case "gez":
                        return "Geez";
                    case "geo":
                        return "Georgian";
                    case "kat":
                        return "Georgian";
                    case "deu":
                        return "German";
                    case "ger":
                        return "German";
                    case "gmh":
                        return "German, Middle High (ca. 1050-1500)";
                    case "goh":
                        return "German, Old High (ca. 750-1050)";
                    case "gem":
                        return "Germanic (Other)";
                    case "gil":
                        return "Gilbertese";
                    case "gon":
                        return "Gondi";
                    case "got":
                        return "Gothic";
                    case "grb":
                        return "Grebo";
                    case "grc":
                        return "Greek, Ancient (to 1453)";
                    case "ell":
                        return "Greek, Modern (1453-)";
                    case "gre":
                        return "Greek, Modern (1453-)";
                    case "kal":
                        return "Greenlandic";
                    case "grn":
                        return "Guarani";
                    case "guj":
                        return "Gujarati";
                    case "hai":
                        return "Haida";
                    case "hau":
                        return "Hausa";
                    case "haw":
                        return "Hawaiian";
                    case "heb":
                        return "Hebrew";
                    case "her":
                        return "Herero";
                    case "hil":
                        return "Hiligaynon";
                    case "him":
                        return "Himachali";
                    case "hin":
                        return "Hindi";
                    case "hmo":
                        return "Hiri Motu";
                    case "hun":
                        return "Hungarian";
                    case "hup":
                        return "Hupa";
                    case "iba":
                        return "Iban";
                    case "ice":
                        return "Icelandic";
                    case "ibo":
                        return "Igbo";
                    case "ijo":
                        return "Ijo";
                    case "ilo":
                        return "Iloko";
                    case "inc":
                        return "Indic (Other)";
                    case "ine":
                        return "Indo-European (Other)";
                    case "ind":
                        return "Indonesian";
                    case "ina":
                        return "Interlingua (International Auxiliary language Association)";

                    // case ("ine"): return "Interlingue";
                    case "iku":
                        return "Inuktitut";
                    case "ipk":
                        return "Inupiak";
                    case "ira":
                        return "Iranian (Other)";
                    case "gai":
                        return "Irish";
                    case "iri":
                        return "Irish";
                    case "sga":
                        return "Irish, Old (to 900)";
                    case "mga":
                        return "Irish, Middle (900 - 1200)";
                    case "iro":
                        return "Iroquoian languages";
                    case "ita":
                        return "Italian";
                    case "jpn":
                        return "Japanese";
                    case "jav":
                        return "Javanese";
                    case "jaw":
                        return "Javanese";
                    case "jrb":
                        return "Judeo-Arabic";
                    case "jpr":
                        return "Judeo-Persian";
                    case "kab":
                        return "Kabyle";
                    case "kac":
                        return "Kachin";
                    case "kam":
                        return "Kamba";
                    case "kan":
                        return "Kannada";
                    case "kau":
                        return "Kanuri";
                    case "kaa":
                        return "Kara-Kalpak";
                    case "kar":
                        return "Karen";
                    case "kas":
                        return "Kashmiri";
                    case "kaw":
                        return "Kawi";
                    case "kaz":
                        return "Kazakh";
                    case "kha":
                        return "Khasi";
                    case "khm":
                        return "Khmer";
                    case "khi":
                        return "Khoisan (Other)";
                    case "kho":
                        return "Khotanese";
                    case "kik":
                        return "Kikuyu";
                    case "kin":
                        return "Kinyarwanda";
                    case "kir":
                        return "Kirghiz";
                    case "kom":
                        return "Komi";
                    case "kon":
                        return "Kongo";
                    case "kok":
                        return "Konkani";
                    case "kor":
                        return "Korean";
                    case "kpe":
                        return "Kpelle";
                    case "kro":
                        return "Kru";
                    case "kua":
                        return "Kuanyama";
                    case "kum":
                        return "Kumyk";
                    case "kur":
                        return "Kurdish";
                    case "kru":
                        return "Kurukh";
                    case "kus":
                        return "Kusaie";
                    case "kut":
                        return "Kutenai";
                    case "lad":
                        return "Ladino";
                    case "lah":
                        return "Lahnda";
                    case "lam":
                        return "Lamba";
                    case "oci":
                        return "Langue d'Oc (post 1500)";
                    case "lao":
                        return "Lao";
                    case "lat":
                        return "Latin";
                    case "lav":
                        return "Latvian";
                    case "ltz":
                        return "Letzeburgesch";
                    case "lez":
                        return "Lezghian";
                    case "lin":
                        return "Lingala";
                    case "lit":
                        return "Lithuanian";
                    case "loz":
                        return "Lozi";
                    case "lub":
                        return "Luba-Katanga";
                    case "lui":
                        return "Luiseno";
                    case "lun":
                        return "Lunda";
                    case "luo":
                        return "Luo (Kenya and Tanzania)";
                    case "mac":
                        return "Macedonian";
                    case "mad":
                        return "Madurese";
                    case "mag":
                        return "Magahi";
                    case "mai":
                        return "Maithili";
                    case "mak":
                        return "Makasar";
                    case "mlg":
                        return "Malagasy";
                    case "may":
                        return "Malay";
                    case "msa":
                        return "Malay";
                    case "mal":
                        return "Malayalam";
                    case "mlt":
                        return "Maltese";
                    case "man":
                        return "Mandingo";
                    case "mni":
                        return "Manipuri";
                    case "mno":
                        return "Manobo languages";
                    case "max":
                        return "Manx";
                    case "mao":
                        return "Maori";
                    case "mri":
                        return "Maori";
                    case "mar":
                        return "Marathi";
                    case "chm":
                        return "Mari";
                    case "mah":
                        return "Marshall";
                    case "mwr":
                        return "Marwari";
                    case "mas":
                        return "Masai";
                    case "myn":
                        return "Mayan languages";
                    case "men":
                        return "Mende";
                    case "mic":
                        return "Micmac";
                    case "min":
                        return "Minangkabau";
                    case "mis":
                        return "Miscellaneous (Other)";
                    case "moh":
                        return "Mohawk";
                    case "mol":
                        return "Moldavian";
                    case "mkh":
                        return "Mon-Kmer (Other)";
                    case "lol":
                        return "Mongo";
                    case "mon":
                        return "Mongolian";
                    case "mos":
                        return "Mossi";
                    case "mul":
                        return "Multiple languages";
                    case "mun":
                        return "Munda languages";
                    case "nau":
                        return "Nauru";
                    case "nav":
                        return "Navajo";
                    case "nde":
                        return "Ndebele, North";
                    case "nbl":
                        return "Ndebele, South";
                    case "ndo":
                        return "Ndongo";
                    case "nep":
                        return "Nepali";
                    case "new":
                        return "Newari";
                    case "nic":
                        return "Niger-Kordofanian (Other)";
                    case "ssa":
                        return "Nilo-Saharan (Other)";
                    case "niu":
                        return "Niuean";
                    case "non":
                        return "Norse, Old";
                    case "nai":
                        return "North American Indian (Other)";
                    case "nor":
                        return "Norwegian";
                    case "nob":
                        return "Norwegian (Bokmål)";
                    case "nno":
                        return "Norwegian (Nynorsk)";
                    case "nub":
                        return "Nubian languages";
                    case "nym":
                        return "Nyamwezi";
                    case "nya":
                        return "Nyanja";
                    case "nyn":
                        return "Nyankole";
                    case "nyo":
                        return "Nyoro";
                    case "nzi":
                        return "Nzima";
                    case "oji":
                        return "Ojibwa";
                    case "ori":
                        return "Oriya";
                    case "orm":
                        return "Oromo";
                    case "osa":
                        return "Osage";
                    case "oss":
                        return "Ossetic";
                    case "oto":
                        return "Otomian languages";
                    case "pal":
                        return "Pahlavi";
                    case "pau":
                        return "Palauan";
                    case "pli":
                        return "Pali";
                    case "pam":
                        return "Pampanga";
                    case "pag":
                        return "Pangasinan";
                    case "pan":
                        return "Panjabi";
                    case "pap":
                        return "Papiamento";
                    case "paa":
                        return "Papuan-Australian (Other)";
                    case "fas":
                        return "Persian";
                    case "per":
                        return "Persian";
                    case "peo":
                        return "Persian, Old (ca 600 - 400 B.C.)";
                    case "phn":
                        return "Phoenician";
                    case "pol":
                        return "Polish";
                    case "pon":
                        return "Ponape";
                    case "por":
                        return "Portuguese";
                    case "pra":
                        return "Prakrit languages";
                    case "pro":
                        return "Provencal, Old (to 1500)";
                    case "pus":
                        return "Pushto";
                    case "que":
                        return "Quechua";
                    case "roh":
                        return "Rhaeto-Romance";
                    case "raj":
                        return "Rajasthani";
                    case "rar":
                        return "Rarotongan";
                    case "roa":
                        return "Romance (Other)";
                    case "ron":
                        return "Romanian";
                    case "rum":
                        return "Romanian";
                    case "rom":
                        return "Romany";
                    case "run":
                        return "Rundi";
                    case "rus":
                        return "Russian";
                    case "sal":
                        return "Salishan languages";
                    case "sam":
                        return "Samaritan Aramaic";
                    case "smi":
                        return "Sami languages";
                    case "smo":
                        return "Samoan";
                    case "sad":
                        return "Sandawe";
                    case "sag":
                        return "Sango";
                    case "san":
                        return "Sanskrit";
                    case "srd":
                        return "Sardinian";
                    case "sco":
                        return "Scots";
                    case "sel":
                        return "Selkup";
                    case "sem":
                        return "Semitic (Other)";
                    case "srp":
                        return "Serbian";
                    case "scr":
                        return "Serbo-Croatian";
                    case "srr":
                        return "Serer";
                    case "shn":
                        return "Shan";
                    case "sna":
                        return "Shona";
                    case "sid":
                        return "Sidamo";
                    case "bla":
                        return "Siksika";
                    case "snd":
                        return "Sindhi";
                    case "sin":
                        return "Singhalese";
                    case "sit":
                        return "Sino-Tibetan (Other)";
                    case "sio":
                        return "Siouan languages";
                    case "sla":
                        return "Slavic (Other)";

                    // case ("ssw"): return "Siswant";
                    case "slk":
                        return "Slovak";
                    case "slv":
                        return "Slovenian";
                    case "sog":
                        return "Sogdian";
                    case "som":
                        return "Somali";
                    case "son":
                        return "Songhai";
                    case "wen":
                        return "Sorbian languages";
                    case "nso":
                        return "Sotho, Northern";
                    case "sot":
                        return "Sotho, Southern";
                    case "sai":
                        return "South American Indian (Other)";
                    case "esl":
                        return "Spanish";
                    case "spa":
                        return "Spanish";
                    case "suk":
                        return "Sukuma";
                    case "sux":
                        return "Sumerian";
                    case "sun":
                        return "Sudanese";
                    case "sus":
                        return "Susu";
                    case "swa":
                        return "Swahili";
                    case "ssw":
                        return "Swazi";
                    case "sve":
                        return "Swedish";
                    case "swe":
                        return "Swedish";
                    case "syr":
                        return "Syriac";
                    case "tgl":
                        return "Tagalog";
                    case "tah":
                        return "Tahitian";
                    case "tgk":
                        return "Tajik";
                    case "tmh":
                        return "Tamashek";
                    case "tam":
                        return "Tamil";
                    case "tat":
                        return "Tatar";
                    case "tel":
                        return "Telugu";
                    case "ter":
                        return "Tereno";
                    case "tha":
                        return "Thai";
                    case "bod":
                        return "Tibetan";
                    case "tib":
                        return "Tibetan";
                    case "tig":
                        return "Tigre";
                    case "tir":
                        return "Tigrinya";
                    case "tem":
                        return "Timne";
                    case "tiv":
                        return "Tivi";
                    case "tli":
                        return "Tlingit";
                    case "tog":
                        return "Tonga (Nyasa)";
                    case "ton":
                        return "Tonga (Tonga Islands)";
                    case "tru":
                        return "Truk";
                    case "tsi":
                        return "Tsimshian";
                    case "tso":
                        return "Tsonga";
                    case "tsn":
                        return "Tswana";
                    case "tum":
                        return "Tumbuka";
                    case "tur":
                        return "Turkish";
                    case "ota":
                        return "Turkish, Ottoman (1500 - 1928)";
                    case "tuk":
                        return "Turkmen";
                    case "tyv":
                        return "Tuvinian";
                    case "twi":
                        return "Twi";
                    case "uga":
                        return "Ugaritic";
                    case "uig":
                        return "Uighur";
                    case "ukr":
                        return "Ukrainian";
                    case "umb":
                        return "Umbundu";
                    case "und":
                        return "Undetermined";
                    case "urd":
                        return "Urdu";
                    case "uzb":
                        return "Uzbek";
                    case "vai":
                        return "Vai";
                    case "ven":
                        return "Venda";
                    case "vie":
                        return "Vietnamese";
                    case "vol":
                        return "Volapük";
                    case "vot":
                        return "Votic";
                    case "wak":
                        return "Wakashan languages";
                    case "wal":
                        return "Walamo";
                    case "war":
                        return "Waray";
                    case "was":
                        return "Washo";
                    case "cym":
                        return "Welsh";
                    case "wel":
                        return "Welsh";
                    case "wol":
                        return "Wolof";
                    case "xho":
                        return "Xhosa";
                    case "sah":
                        return "Yakut";
                    case "yao":
                        return "Yao";
                    case "yap":
                        return "Yap";
                    case "yid":
                        return "Yiddish";
                    case "yor":
                        return "Yoruba";
                    case "zap":
                        return "Zapotec";
                    case "zen":
                        return "Zenaga";
                    case "zha":
                        return "Zhuang";
                    case "zul":
                        return "Zulu";
                    case "zun":
                        return "Zuni";
                }

                return "Any";
            }
        }
    }
}