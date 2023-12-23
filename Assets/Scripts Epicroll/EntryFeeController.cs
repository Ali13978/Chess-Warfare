using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

namespace mmocircle
{
    public class EntryFeeController : MonoBehaviour
    {
        public static EntryFeeController instance;

        public static float currentEntryFee = 0.1f;
        public static float currentRewardPrice = 0.3f;
        public static double Rpvalue_ = 0f;

        [SerializeField] private string address = "https://wgcapi.mmocircles.com/market/rates";
        [SerializeField] Text[] EntryTextToChange, RewardTextToChange;

        private void Start()
        {
            if (instance == null) instance = this;
            else Destroy(this);
            CheckEntryFee();
        }


        void CheckEntryFee() => StartCoroutine(SendForm(address, HandleResponse));

        IEnumerator SendForm(string webAddress, Action<string> funtionToSend)
        {
            using (UnityWebRequest webRequest = UnityWebRequest.Get(webAddress))
            {
                webRequest.certificateHandler = null;
                yield return webRequest.SendWebRequest();
                var result = webRequest.downloadHandler.text;

                if (!result.Contains("ConnectionError"))
                    funtionToSend(webRequest.downloadHandler.text);
            }
        }

        void HandleResponse(string json)
        {
            ServerResponse response = JsonUtility.FromJson<ServerResponse>(json);
            if (response == null) return;
            currentEntryFee = 0.3f / float.Parse(response.sgc);
            ChangeEntryFeeText(currentEntryFee.ToString());
            ChangeRewardFeeText(currentEntryFee);


            
            double totalvalue = currentEntryFee;
            int loserscount = 2; // if 2 players  //new
            double serverCut = Math.Round((totalvalue * loserscount) * 0.15f,2);
            double userreward = Math.Round(((totalvalue * loserscount) * 0.85f),2);   

            double RP_only_1_winner = Math.Round(Math.Log((double)(serverCut * totalvalue * 30) + Math.E) * 100 + Math.Log((double)userreward + Math.E) * 250 - 350, 3);
            Rpvalue_ = Math.Round(RP_only_1_winner, 1);



        }

        private void ChangeEntryFeeText(string text)
        {
            foreach (var item in EntryTextToChange)
            {
                item.text = text;
            }
        }

        private void ChangeRewardFeeText(float currentFee)
        {
            currentRewardPrice = CalculatePercentage(currentFee);
            foreach (var item in RewardTextToChange)
            {
                item.text = currentRewardPrice.ToString();
            }
        }

        private float CalculatePercentage(float amount)
        {
            amount = amount * 2;
            amount = (85f / 100) * amount;
            return amount;
        }
    }

    [Serializable]
    public class ServerResponse
    {
        public string sgc;
        public string aud;
    }
}