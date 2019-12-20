using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Enemy : MonoBehaviour {

    public int Hp = 15;
    public int MaxHp = 30;
    public int MinHp = 0;
    public Text HpText;
    public Image HurtImg;
    public Text HurtText;
    float time = 0;//图片文字显示时间

    public GameObject WinPanel;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {

        if(HurtImg.enabled == true)
        {
            time += Time.deltaTime;
            if(time>1f)
            {
                HurtImg.enabled = false;
                HurtText.enabled = false;
                time = 0;
            }
        }

    }

    public void Hurt(int Att)
    {
        Hp -= Att;
        HurtImg.enabled = true;
        HurtText.enabled = true;
        HurtText.text = "-" + Att.ToString();
        if (Hp <= MinHp)
        {
            Hp = MinHp;
            WinPanel.SetActive(true);
            WinPanel.GetComponent<Animator>().enabled = true;
            GameObject.Find("WinImage").GetComponent<Image>().sprite = VS._instance.PlayerImg.sprite;
            Camera.main.GetComponent<AudioSource>().Stop();
            GameObject.Find("WinPanel").GetComponent<AudioSource>().Play();
        }
        HpText.text = Hp.ToString();
    }

    public void Blood(int blood)
    {
        Hp += blood;
        if (Hp >= MaxHp)
        {
            Hp = MaxHp;
        }
        HpText.text = Hp.ToString();
    }

}
