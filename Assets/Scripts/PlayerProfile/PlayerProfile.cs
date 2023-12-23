[System.Serializable]
public class PlayerProfile
{ //keep variable names short to reduce size on Firebase Database

    public string nm = ""; //Player Name
    public string UID = ""; //User ID
    
    public string C = "pk"; //Country

    //total stats of the player
    public int TW = 0; //Total Wins
    public int TL = 0; //Total Losses

    public int Lvl = 1; //Level
    public string av = "male_Cat1"; //avatar id current (default)

    public PD pD=new PD(); //private Details
}