using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class Skill : MonoBehaviour {

    public Image FNImg;
    public GameObject ZHImg;
    public Text CFText;
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void FengNu()
    {
        FNImg.GetComponent<Image>().enabled = true;
        RectTransform rt = FNImg.rectTransform;//获取透明度
        Color c = FNImg.color;
        c.a = 0;
        FNImg.color = c;
        Sequence sequence = DOTween.Sequence();
        Tweener a1 = FNImg.DOColor(new Color(c.r, c.g, c.b, 1), 1);//风怒图出现
        Tweener a2 = FNImg.DOColor(new Color(c.r, c.g, c.b, 0), 1);
        sequence.Append(a1);
        sequence.AppendInterval(0.5f);
        sequence.Append(a2);
        StartCoroutine(FengNuDelay());
    }

    public void ZhanHong()
    {
        ZHImg.SetActive(true);
        Sequence sequence = DOTween.Sequence();
        Tweener s1 = ZHImg.transform.DOScale(new Vector3(1.5f, 1.5f, 1), 0.5f);
        Tweener s2 = ZHImg.transform.DOScale(new Vector3(1, 1, 1), 0.5f);
        sequence.Append(s1);
        sequence.AppendInterval(0.3f);
        sequence.Append(s2);
        StartCoroutine(ZhanHongDelay());
    }

    public void ChaoFeng()
    {


    }

    IEnumerator ZhanHongDelay()
    {
        yield return new WaitForSeconds(1.3f);
        GameObject.Find("ImagePlayer").GetComponent<Player>().Blood(2);
        ZHImg.SetActive(false);
    }

    IEnumerator FengNuDelay()
    {
        yield return new WaitForSeconds(2.5f);
        FNImg.GetComponent<Image>().enabled = false;
        Sequence sequence = DOTween.Sequence();
        Tweener m1 = Game._instance.Aggressor.transform.DOMove(Game._instance.ToAggressor.transform.position, 0.1f);
        Tweener m2 = Game._instance.Aggressor.transform.DOMove(Game._instance.Aggressor.transform.position, 0.4f);
        Tweener r1 = Game._instance.ToAggressor.transform.DORotate(new Vector3(0, 0, -5), 0.3f);
        Tweener r2 = Game._instance.ToAggressor.transform.DORotate(new Vector3(0, 0, 5), 0.5f);
        Tweener r3 = Game._instance.ToAggressor.transform.DORotate(new Vector3(0, 0, 0), 0.3f);
        sequence.Append(m1);
        sequence.Append(m2);
        sequence.Append(r1);
        sequence.Append(r2);
        sequence.Append(r3);
        Game._instance.ToAggressor.GetComponent<Enemy>().
            Hurt(Game._instance.Aggressor.GetComponent<Card>().Attack);
    }

    

}
