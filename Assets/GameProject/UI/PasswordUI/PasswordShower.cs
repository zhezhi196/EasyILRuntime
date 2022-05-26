using Module;
using UnityEngine;
using UnityEngine.UI;

namespace Chapter2.UI
{
    public class PasswordShower: MonoBehaviour
    {
        public Text[] inputText;

        public void Input(string put)
        {
            for (int i = 0; i < inputText.Length; i++)
            {
                if (i < put.Length)
                {
                    inputText[i].gameObject.OnActive(true);
                    inputText[i].text = put[i].ToString();
                }
                else
                {
                    inputText[i].gameObject.OnActive(false);
                }
            }
        }
    }
}