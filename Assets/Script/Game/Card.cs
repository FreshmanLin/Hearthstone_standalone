using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using DG.Tweening;


public class Card : MonoBehaviour,IPointerClickHandler {


    GameObject go;
    bool SureBool;
    float time = 0;
    public int ConsumeNum;
    public int Attack;
    public int Hp;
    public int index;
    public Text HpText;

    public Image HurtImg;
    public Text HurtText;

    float TXtime = 0;
    float hurtTime = 0;
    public int SkillType;
    public bool isMagic;

    public AudioSource ComeOnAS;
    public AudioSource SkillAS;
    public AudioSource AttAS;



    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.pointerEnter.tag=="ShouPai")
        {
            for (int i = 0; i < Game._instance.ShouPaiArray.Count; i++)
            {
                Game._instance.ShouPaiArray[i].GetComponent<Image>().enabled = false;
                Game._instance.ShouPaiArray[i].GetChild(0).tag = "ShouPai";
            }

            go = eventData.pointerEnter;
            int Index = go.GetComponent<Card>().index;//得到牌在牌库中的序号
            Debug.Log("Card"+Index);
            SureBool = true;
            go.transform.parent.DOMoveY(eventData.pointerEnter.transform.parent.position.y + 20, 0.2f);
        eventData.pointerEnter.transform.parent.GetComponent<Image>().enabled = true;
            go.tag = "ZB";
            time = 0;
            
        }

        if (EventSystem.current.IsPointerOverGameObject() == false)
        {
            SureBool = false;
            go.transform.parent.DOMoveY(eventData.pointerEnter.transform.parent.position.y - 20, 0.2f);
            eventData.pointerEnter.transform.parent.GetComponent<Image>().enabled = false;
            go.tag = "ShouPai";
            time = 0;
        }

        if (eventData.pointerEnter.tag == "ZB" && time >= 0.5f & ConsumeNum <= Game._instance.AtCryaralNum)
        {
            //go.transform.parent = GameObject.Find("Canvas").transform;
            Camera.main.GetComponent<Game>().ReduceCrystal(ConsumeNum);

            Sequence sequence = DOTween.Sequence();
            Tweener m1 = go.transform.parent.DOMoveY(eventData.pointerEnter.transform.parent.position.y + 297, 0.5f);
            Tweener m2 = go.transform.parent.DOMoveY(eventData.pointerEnter.transform.parent.position.y + 377, 0.5f);
            Tweener s1 = go.transform.parent.DOScale(new Vector3(1f, 0.8f, 1), 0.5f);
            Tweener s2 = go.transform.parent.DOScale(new Vector3(0.5f, 0.4f, 1), 0.5f);
            sequence.Append(m1);
            sequence.Join(s1);
            sequence.AppendInterval(0.5f);
            sequence.Append(m2);
            sequence.Join(s2);
            StartCoroutine(ComeCardDelay());
            SureBool = false;
            time = 0;
        }
        if (eventData.pointerEnter.tag == "ZB" && time >= 0.5f &&
            ConsumeNum > Game._instance.AtCryaralNum)
        {
            Game._instance.HintText.text = "水晶不足";
            Game._instance.HintText.enabled = true;
        }


        if (eventData.pointerEnter.tag == "ZhuoPai")
        {           
            Game._instance.Aggressor = eventData.pointerEnter;
            int AggressorIndex = Game._instance.Aggressor.GetComponent<Card>().index;//准备攻击的卡牌序号
            Debug.Log(AggressorIndex);
        }
        if(eventData.pointerEnter.tag=="Enemy")
        {
            Game._instance.ToAggressor = eventData.pointerEnter;
            
            Sequence sequence = DOTween.Sequence();
            Tweener m1 = Game._instance.Aggressor.transform.DOMove(Game._instance.ToAggressor.transform.position, 0.1f);
            Tweener m2 = Game._instance.Aggressor.transform.DOMove(Game._instance.Aggressor.transform.position, 0.4f);
            Tweener r1 = Game._instance.EnemyImg.transform.DORotate(new Vector3(0, 0, -5), 0.3f);
            Tweener r2 = Game._instance.EnemyImg.transform.DORotate(new Vector3(0, 0, 5), 0.5f);
            Tweener r3 = Game._instance.EnemyImg.transform.DORotate(new Vector3(0, 0, 0), 0.3f);
            sequence.Append(m1);
            sequence.Append(m2);
            sequence.Append(r1);
            sequence.Append(r2);
            sequence.Append(r3);
            StartCoroutine(DelayHurt());
            Game._instance.Aggressor.GetComponent<Card>().AttAS.Play();
            
        }
        if (eventData.pointerEnter.tag == "EnemyCard")
        {
            
            Game._instance.ToAggressor = eventData.pointerEnter;
            if (Game._instance.hasTaunt==false|| Game._instance.hasTaunt == true && Game._instance.ToAggressor.GetComponent<Card>().SkillType == 3)
            {
                int ToAggressorIndex = Game._instance.ToAggressor.GetComponent<Card>().index;//敌方卡牌序号
                Debug.Log(ToAggressorIndex);
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
                StartCoroutine(DelayHurtCard());
                Game._instance.Aggressor.GetComponent<Card>().AttAS.Play();
            }
            else
            {
                Game._instance.HintText.text = "请攻击具有嘲讽的随从";
                Game._instance.HintText.enabled = true;
                release();
            }
            
        }
        if(eventData.pointerEnter.tag == "AlreadyAtt")
        {

            Game._instance.HintText.text = "该随从已经攻击过了";
            Game._instance.HintText.enabled = true;
        }
        
    }

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        if (SureBool == true)
        {
            time += Time.deltaTime;
        }
        if(go==null)
        {
            return;
        }
        if (go.tag == "ZhuoPai")
        {
            TXtime += Time.deltaTime;
            if(TXtime>=2)
            {
                go.transform.parent.GetComponent<Image>().enabled = false;
            }
        }

        if (HurtImg.enabled == true)
        {
            hurtTime += Time.deltaTime;
            if (hurtTime > 1f)
            {
                HurtImg.enabled = false;
                HurtText.enabled = false;
                hurtTime = 0;
            }
        }

        
	}

    void Grave()
    {
        Game._instance.Aggressor.GetComponent<Card>().HurtImg.enabled = false;
        Game._instance.Aggressor.GetComponent<Card>().HurtText.enabled = false;
        Game._instance.Aggressor.transform.SetParent(GameObject.Find("Grave").transform);
        Game._instance.Aggressor.transform.position = Vector2.zero;
        Destroy(Game._instance.Aggressor.GetComponent<Card>());//删除进入废牌区的牌的脚本
        Camera.main.GetComponent<Game>().GraveManage();
        Debug.Log("进入废牌区");
    }

    IEnumerator ComeCardDelay()
    {
        yield return new WaitForSeconds(1.4f);
        go.transform.parent.parent = GameObject.Find("ZhuoPanel").transform;
        go.tag = "ZhuoPai";
        go.GetComponent<Card>().ComeOnAS.Play();
        go.transform.parent.GetComponent<Image>().sprite = Game._instance.Pos.GetComponent<Image>().sprite;
        Game._instance.ShouPaiArray.Remove(go.transform.parent);
        Game._instance.num--;
        Game._instance.ZhuoPaiArray.Add(go.transform.parent);
        if (go.GetComponent<Card>().SkillType == 2)
        {
            Camera.main.GetComponent<Skill>().ZhanHong();
            go.GetComponent<Card>().SkillAS.Play();
        }
        if(go.GetComponent<Card>().SkillType == 3)
        {
            Game._instance.myTaunt = true;
        }
    }

    IEnumerator DelayHurt()
    {
        yield return new WaitForSeconds(0.5f);
        Game._instance.ToAggressor.GetComponent<Enemy>().Hurt(Game._instance.Aggressor.GetComponent<Card>().Attack);
        Debug.Log("攻击敌方英雄完成");
        switch (Game._instance.Aggressor.GetComponent<Card>().SkillType)
        {
            case 1:
                Camera.main.GetComponent<Skill>().FengNu();
                Game._instance.Aggressor.GetComponent<Card>().SkillAS.Play();
                break;
        }
        Game._instance.Aggressor.tag = "AlreadyAtt";
        Debug.Log("记录已经攻击过一次了");
        StartCoroutine(release());
        //Game._instance.Aggressor = null;
        //Grave();
    }

    public void CardHurt(int Att)
    {
        Debug.Log("执行了一次CardHurt");
        Hp -= Att;
        HurtImg.enabled = true;
        HurtText.enabled = true;
        HurtText.text = "-" + Att.ToString();
        hurtTime += Time.deltaTime;
        StartCoroutine(hide());
        if (Hp <= 0)
        {
            
            //Hp = MinHp;

        }
        HpText.text = Hp.ToString();
    }
    IEnumerator toGrave()
    {
        yield return new WaitForSeconds(1f);
        Grave();
    }

    IEnumerator hide()
    {
        yield return new WaitForSeconds(1f);
        if (HurtImg.enabled == true)
        {
            HurtImg.enabled = false;
            HurtText.enabled = false;
            hurtTime = 0;
        }
    }

    IEnumerator release()
    {
        yield return new WaitForSeconds(0.5f);
        Game._instance.Aggressor = null;
    }

    IEnumerator DelayHurtCard()
    {
        yield return new WaitForSeconds(0.5f);
        Game._instance.ToAggressor.GetComponent<Card>().CardHurt(Game._instance.Aggressor.GetComponent<Card>().Attack);
        if (Game._instance.ToAggressor.GetComponent<Card>().Hp <= 0)
        {
            //Game._instance.ToAggressor.SetActive(false);
            Destroy(Game._instance.ToAggressor);
            Game._instance.EnemyZhuoArray.Remove(Game._instance.ToAggressor.transform);
        }

        Game._instance.Aggressor.GetComponent<Card>().CardHurt(Game._instance.ToAggressor.GetComponent<Card>().Attack);
        if (Game._instance.Aggressor.GetComponent<Card>().Hp <= 0)
        {
            GameObject ca =  Game._instance.Aggressor.transform.parent.gameObject;
            ca.SetActive(false);
            StartCoroutine(toGrave());
            //DestroyImmediate(Game._instance.Aggressor);
        }
        Debug.Log("攻击随从完成");
        switch (Game._instance.Aggressor.GetComponent<Card>().SkillType)
        {
            case 1:
                Camera.main.GetComponent<Skill>().FengNu();
                Game._instance.Aggressor.GetComponent<Card>().SkillAS.Play();
                break;
        }
        Game._instance.Aggressor.tag = "AlreadyAtt";
        Debug.Log("记录已经攻击过一次了");
        StartCoroutine(release());
        //Game._instance.Aggressor = null;
    }

}
