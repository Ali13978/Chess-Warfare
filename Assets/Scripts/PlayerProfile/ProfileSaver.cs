using System.IO;
using System.Xml.Serialization;
using UnityEngine;

public class ProfileSaver
{
    public void SaveProfile(PlayerProfile playerProfile)
    {
        XmlSerializer serializer = new XmlSerializer(typeof(PlayerProfile));
        using (StringWriter stringWriter = new StringWriter())
        {
            serializer.Serialize(stringWriter, playerProfile);
            PlayerPrefs.SetString("PlayerProfile", stringWriter.ToString());
        }
    }

    public PlayerProfile LoadProfile()
    {
        XmlSerializer serializer = new XmlSerializer(typeof(PlayerProfile));
        string text = PlayerPrefs.GetString("PlayerProfile");
        if (text.Length != 0)
        {
            using (var reader = new System.IO.StringReader(text))
            {
                return serializer.Deserialize(reader) as PlayerProfile;
            }
        }
        return null;
    }

    public void SaveMiniGames(MiniGame miniGame)
    {
        XmlSerializer serializer = new XmlSerializer(typeof(MiniGame));
        using (StringWriter stringWriter = new StringWriter())
        {
            serializer.Serialize(stringWriter, miniGame);
            PlayerPrefs.SetString("minigame", stringWriter.ToString());
        }
    }

    public MiniGame LoadMiniGames()
    {
        XmlSerializer serializer = new XmlSerializer(typeof(MiniGame));
        string text = PlayerPrefs.GetString("minigame");
        if (text.Length != 0)
        {
            using (var reader = new System.IO.StringReader(text))
            {
                return serializer.Deserialize(reader) as MiniGame;
            }
        }
        return null;
    }

    public void SaveMyBoards(MyBoards myBoards)
    {
        XmlSerializer serializer = new XmlSerializer(typeof(MyBoards));
        using (StringWriter stringWriter = new StringWriter())
        {
            serializer.Serialize(stringWriter, myBoards);
            PlayerPrefs.SetString("MyBoards", stringWriter.ToString());
        }
    }

    public MyBoards LoadMyBoards()
    {
        XmlSerializer serializer = new XmlSerializer(typeof(MyBoards));
        string text = PlayerPrefs.GetString("MyBoards");
        if (text.Length != 0)
        {
            using (var reader = new System.IO.StringReader(text))
            {
                return serializer.Deserialize(reader) as MyBoards;
            }
        }
        return null;
    }

    public void SaveMyPacks(MyPacks myPacks)
    {
        XmlSerializer serializer = new XmlSerializer(typeof(MyPacks));
        using (StringWriter stringWriter = new StringWriter())
        {
            serializer.Serialize(stringWriter, myPacks);
            PlayerPrefs.SetString("MyPacks", stringWriter.ToString());
        }
    }

    public MyPacks LoadMyPacks()
    {
        XmlSerializer serializer = new XmlSerializer(typeof(MyPacks));
        string text = PlayerPrefs.GetString("MyPacks");
        if (text.Length != 0)
        {
            using (var reader = new System.IO.StringReader(text))
            {
                return serializer.Deserialize(reader) as MyPacks;
            }
        }
        return null;
    }

    //profileSaver.SaveClass<MyAvatars>(myAvatars, "MyAvatars");
    public void SaveClass<T>(T AClass,string ClassName)
    {
        XmlSerializer serializer = new XmlSerializer(typeof(T));
        using (StringWriter stringWriter = new StringWriter())
        {
            serializer.Serialize(stringWriter, AClass);
            PlayerPrefs.SetString(ClassName, stringWriter.ToString());
        }
    }

    public T LoadClass<T>(string ClassName)
    {
        XmlSerializer serializer = new XmlSerializer(typeof(T));
        string text = PlayerPrefs.GetString(ClassName);
        if (text.Length != 0)
        {
            using (var reader = new System.IO.StringReader(text))
            {
                return (T)serializer.Deserialize(reader);
            }
        }
        return default(T);
    }
}
