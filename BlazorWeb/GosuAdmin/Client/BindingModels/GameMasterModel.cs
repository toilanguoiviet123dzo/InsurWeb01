using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace GosuAdmin.Client.BindingModels
{
    public class GameMasterModel
    {
        public string ID { get; set; } = "";
        public string GameID { get; set; } = "";
        public string GameName { get; set; } = "";
        public string GameIdentityName { get; set; } = "";
        public int GameStatus { get; set; }
        public string GameGroup { get; set; } = "";
        public string IconImage { get; set; } = "";
        public string TitleImage { get; set; } = "";
        public int SortOrder { get; set; }
        public string HomeLink { get; set; } = "";
        public string ForumLink { get; set; } = "";
        public bool ServerRequired { get; set; }
        public bool CardPayEnabled { get; set; }
        public string CardPayDesc { get; set; } = "";
        public string GosuTransferType { get; set; } = "";
        public string GosuTransferDesc { get; set; } = "";
        public double ExchangeRate { get; set; }
        public string GameCurrencyName { get; set; } = "";
        public string ServiceID { get; set; } = "";
        public bool ShopRequired { get; set; }
        public bool CharacterRequired { get; set; }
        public string Channels { get; set; } = "";
        public string Note { get; set; } = "";
        public bool IsCharacterAllServer { get; set; }
        public string DelegateServerID { get; set; } = "";
        public DateTime CreatedOn { get; set; }
        public DateTime ModifiedOn { get; set; }
        public int UpdMode { get; set; }
    }
}