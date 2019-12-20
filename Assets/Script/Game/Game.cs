using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using UnityEngine.EventSystems;

public class Game : MonoBehaviour
{

    public static Game _instance;


    public List<Transform> ShouPaiArray;
    public List<Transform> ZhuoPaiArray;
    public List<Transform> GraveArray;

    public List<Transform> EnemyShouArray;
    public List<Transform> EnemyZhuoArray;

    GameObject Grave;

    public Image PlayerImg;
    public Image EnemyImg;

    public Image[] CrystalArr;
    
    public Text CrystalText;
    public int CrystalNum = 0;
    public int AtCryaralNum;
    public Text HintText;
    float HintTime = 0;

    public GameObject Pos;//卡牌生成位置
    public List<GameObject> CardsObj;//卡牌预制体
    GameObject go;
    GameObject goo;
    GameObject target;
    public GameObject EnemyCardObj;
    public List<GameObject> EnemyCardsObj;

    public Text OverBtnText;
    bool OverBool;
    public Button OverBtn;

    public int num = 0;
    public int EnemyNum = 0;
    int g = 0;

    public Image ClockImg;
    float ClockTime = 0;
    bool ClockBool;
    bool EnemyClockBool;
    public Image ClockTXImg;

    public GameObject Aggressor;
    public GameObject ToAggressor;
    int m = 0;

    public Button Surrender;
    public Button Chat;
    public Button ChatBubble1;
    public Button ChatBubble2;
    public Button ChatBubble3;
    public Button ChatBubble4;
    public Text text1;
    public Text text2;
    public Text text3;
    public Text text4;
    public GameObject Bubble;
    public GameObject EnemyBubble;
    public Text BubleText;

    bool option = false;
    bool display = false;
    bool EnemyDisplay = false;
    float optionTime=0;
    float displayTime=0;
    float EnemyDisplayTime = 0;

    bool firstRound = true;
    public bool hasTaunt = false;
    public bool myTaunt = false;

    public enum Round//代表回合信息
    {
        Player,//玩家回合
        ToCard,//玩家出牌回合
        AI,
        AICard,
        OverRound
    }

    public Round round = Round.Player;//默认玩家回合
    public int RoundNum = 1;
    int c = 0;
    float BuildCardTime = 2.5f;
    
    float RoundTime = 0;

    // Use this for initialization
    void Start () {

        _instance = this;
        Grave = GameObject.Find("Grave");

        ChatBubble1.onClick.AddListener(chat1);
        ChatBubble2.onClick.AddListener(chat2);
        ChatBubble3.onClick.AddListener(chat3);
        ChatBubble4.onClick.AddListener(chat4);


        for (int i = 0; i < 4; i++)
        {
            int s = Random.Range(0, 17);
            EnemyCardObj = EnemyCardsObj[s];
            goo = Instantiate(EnemyCardObj);
            goo.tag = "EnemyCard";//Mine
            EnemyShouArray.Add(goo.transform);
            goo.transform.SetParent(GameObject.Find("EnemyShouPanel").transform);
            if(goo.gameObject.GetComponent<Card>().SkillType == 3)
            {
                hasTaunt = true;
            }
            
        }

       // PlayerImg.sprite = VS._instance.PlayerImg.sprite;
        PlayerImg.transform.DOMove(new Vector3(PlayerImg.transform.position.x + 420,
            PlayerImg.transform.position.y - 325, 0), 1.5f);
        EnemyImg.transform.DOMove(new Vector3(EnemyImg.transform.position.x - 386,
            EnemyImg.transform.position.y + 308, 0), 1.5f);

        AddCrystal();
        //CrystalArr[0].enabled = true;
    }
	
	// Update is called once per frame
	void Update () {

        if(HintText.enabled == true)
        {
            HintTime += Time.deltaTime;
            if(HintTime>2f)
            {
                HintText.enabled = false;
                HintTime = 0;
            }
        }

        if (round == Round.Player)
        {
            this.OverBtn.interactable = false;
            BuildCardTime += Time.deltaTime;
            BuildCard();         
            for (int i = 0; i < ZhuoPaiArray.Count; i++)
            {
                ZhuoPaiArray[i].transform.GetChild(0).tag = "ZhuoPai";
                Debug.Log("变回桌牌");
            }
        }

        if (round == Round.ToCard)
        {
            
            this.OverBtn.interactable = true;
            RoundTime += Time.deltaTime;
            if (RoundTime >= 20)//正式值为50
            {
                Clock();
                ClockBool = true;

            }

        }

        if(round == Round.AI)
        {
            this.OverBtn.interactable = false;
            EnemyClockBool = true;            
            //BuildCardTime += Time.deltaTime;
            BuildEnemyCard();
            int e = Random.Range(0, EnemyShouArray.Count);
            Transform temp = EnemyShouArray[e];
            target = EnemyShouArray[e].gameObject;
            
            target.transform.GetChild(2).GetComponent<Image>().enabled = false;//去除卡背
            target.transform.SetParent(GameObject.Find("Canvas").transform);
            Sequence sequence = DOTween.Sequence();
            Tweener m1 = target.transform.DOMove(new Vector3(971.3f,temp.position.y - 297,0), 0.5f);
            Tweener m2 = target.transform.DOMove(new Vector3(971.3f, temp.position.y - 377, 0), 0.5f);
            //Tweener m1 = EnemyShouArray[e].transform.DOMoveY(temp.position.y - 297, 0.5f);
            //Tweener m2 = EnemyShouArray[e].transform.DOMoveY(temp.position.y - 377, 0.5f);
            Tweener s1 = target.transform.DOScale(new Vector3(1f, 0.8f, 1), 0.5f);
            Tweener s2 = target.transform.DOScale(new Vector3(0.35f, 0.35f, 1), 0.5f);
            sequence.Append(m1);
            sequence.Join(s1);
            sequence.AppendInterval(0.5f);
            sequence.Append(m2);
            sequence.Join(s2);
            StartCoroutine(EnemyDelay());
          
            
            
            //攻击玩家的方法;
            
            round = Round.AICard;
        }

        if(ClockBool==true)
        {
            ClockTime += Time.deltaTime;
            if(ClockTime>=10.2f)
            {
                ClockBool = false;
                ClockImg.GetComponent<Animator>().enabled = false;
                ClockImg.GetComponent<Image>().enabled = false;
                ClockTXImg.GetComponent<Image>().enabled = false;
                OverBtnText.text = "对手回合";
                OverBtn.transform.DORotate(new Vector3(360, 0, 0), 1f, RotateMode.FastBeyond360);
                ClockTime = 0;
                OverBool = !OverBool;
                round = Round.AI;
                RoundTime = 0;
            }
        }

        if(EnemyClockBool == true)
        {
            print("敌方计时开始");
            RoundTime += Time.deltaTime;
            if (RoundTime >= 10f)
            {
                print("敌方计时结束");
                EnemyClockBool = false;
                OverBtnText.text = "结束回合";
                OverBtn.transform.DORotate(new Vector3(360, 0, 0), 1f, RotateMode.FastBeyond360);

                RoundTime = 0;
                round = Round.Player;
                AddCrystal();
                this.OverBtn.interactable = true;
            }
        }
        
        if(option == true)
        {
            optionTime += Time.deltaTime;
            if (optionTime > 3f)
            {
                option = false;
                ChatBubble1.gameObject.SetActive(false);
                ChatBubble2.gameObject.SetActive(false);
                ChatBubble3.gameObject.SetActive(false);
                ChatBubble4.gameObject.SetActive(false);
                optionTime = 0;
            }
        }

        if (display == true)
        {
            displayTime += Time.deltaTime;
            optionTime = 0;
            if (displayTime > 2f)
            {
                display = false;
                Bubble.SetActive(false);
                displayTime = 0;
                              
            }
        }

        if (Input.GetKeyDown(KeyCode.K))
        {
            
            EnemyBubble.SetActive(true);
            EnemyDisplay = true;
            
        }

        if (EnemyDisplay == true)
        {
            EnemyDisplayTime += Time.deltaTime;
            if (EnemyDisplayTime > 2f)
            {
                EnemyDisplay = false;
                EnemyDisplayTime = 0;
                EnemyBubble.SetActive(false);
            }
        }

    }

    void EnemyAtt()
    {
        PlayerImg.GetComponent<Player>().Hurt(EnemyZhuoArray[m].GetComponent<Card>().Attack);
        round = Round.OverRound;
    }

    public void GraveManage()
    {
        if(g>=13)
        {
            foreach(Transform child in Grave.transform)
            {
                GraveArray.Add(child);
            }
            Destroy(GraveArray[0].gameObject);
            foreach(Transform child in Grave.transform)
            {
                GraveArray.Remove(child);
            }
        }
        g++;
    }

    public void OverRound()
    {

        OverBool = !OverBool;

        if (OverBool)
        {
            OverBtnText.text = "对手回合";
            round = Round.AI;
            OverBtn.transform.DORotate(new Vector3(360, 0, 0), 1f,RotateMode.FastBeyond360);//可以转到360以外的角度
            if (ClockBool == true)
            {
                ClockBool = false;
                ClockImg.GetComponent<Animator>().enabled = false;
                ClockImg.GetComponent<Image>().enabled = false;
                ClockTXImg.GetComponent<Image>().enabled = false;
                ClockTime = 0;
                RoundTime = 0;
            }
        }
        
    }

    void Clock()
    {
        ClockImg.GetComponent<Animator>().enabled = true;//激活动画组件
        ClockImg.GetComponent<Image>().enabled = true;//激活图片
        ClockTXImg.GetComponent<Image>().enabled = true;
    }

    public void AddCrystal()
    {
        if (CrystalNum < 10)
        {
            for (int i = 0; i <= CrystalNum; i++)
            {
                CrystalArr[i].enabled = true;
            }
            CrystalNum++;
            AtCryaralNum = CrystalNum;
            CrystalText.text = AtCryaralNum.ToString() + "/" + CrystalNum.ToString();
        }
        else
        {
            CrystalNum = 10;
            AtCryaralNum = CrystalNum;
            CrystalText.text = AtCryaralNum.ToString() + "/" + CrystalNum.ToString();
        }
    }

    public void ReduceCrystal(int num)
    {
        for (int i = 0; i < AtCryaralNum; i++)
        {
            CrystalArr[i].enabled = false;
        }
        for (int i = 0; i < AtCryaralNum - num; i++)
        {
            CrystalArr[i].enabled = true;
        }
        AtCryaralNum -= num;
        CrystalText.text = AtCryaralNum.ToString() + "/" + CrystalNum.ToString();
    }

    public void BuildCard()
    {
        if (num <= 9 && BuildCardTime >= 2.5f)
        {
            int x = Random.Range(0, 17);
            go = Instantiate(CardsObj[x], Pos.transform.position, Quaternion.identity);//生成卡牌
            ShouPaiArray.Add(go.transform);
            go.transform.SetParent(GameObject.Find("Canvas").transform);//卡牌父体为画布，否则和画布并列 不会显示出来
            Sequence sequence = DOTween.Sequence();
            Tweener m1 = go.transform.DOMoveX(go.transform.position.x - 500, 0.7f);
            Tweener m2 = go.transform.DOMove(new Vector3(go.transform.position.x - 1185,
                go.transform.position.y - 460, 0), 0.7f);
            Tweener s1 = go.transform.DOScale(new Vector3(0.5f, 0.4f, 0), 0.7f);
            sequence.Append(m1);//1s 进场
            sequence.AppendInterval(0.7f);//停留一秒让玩家看一下抽的是什么牌
            sequence.Append(m2);//1s 飞向手牌区 同时缩小
            sequence.Join(s1);
            
            print("c:"+c);
            print("round:"+round);
            StartCoroutine(Suspend());
            num++;
            BuildCardTime = 0;
            c++;
            
        }
        if (num > 9)
        {
            Debug.Log("手牌已满！");
        }
    }

    IEnumerator Suspend()
    {
        yield return new WaitForSeconds(2.1f);
        switch (num)
        {
            case 6:
                Vector2 z = GameObject.Find("ShouPanel").GetComponent<GridLayoutGroup>().spacing;
                z.x = -205;
                GameObject.Find("ShouPanel").GetComponent<GridLayoutGroup>().spacing = z;
                break;
            case 7:
                z = GameObject.Find("ShouPanel").GetComponent<GridLayoutGroup>().spacing;
                z.x = -222;
                GameObject.Find("ShouPanel").GetComponent<GridLayoutGroup>().spacing = z;
                break;
            case 8:
                z = GameObject.Find("ShouPanel").GetComponent<GridLayoutGroup>().spacing;
                z.x = -234;
                GameObject.Find("ShouPanel").GetComponent<GridLayoutGroup>().spacing = z;
                break;
            case 9:
                z = GameObject.Find("ShouPanel").GetComponent<GridLayoutGroup>().spacing;
                z.x = -241;
                GameObject.Find("ShouPanel").GetComponent<GridLayoutGroup>().spacing = z;
                break;
            case 10:
                z = GameObject.Find("ShouPanel").GetComponent<GridLayoutGroup>().spacing;
                z.x = -248;
                GameObject.Find("ShouPanel").GetComponent<GridLayoutGroup>().spacing = z;
                break;
        }
        if (c >= 3)
        {
            round = Round.ToCard;
            Debug.Log("出牌阶段");
        }
        go.transform.SetParent(GameObject.Find("ShouPanel").transform);
        go.transform.GetChild(0).tag = "ShouPai";
    }
    IEnumerator EnemyDelay()
    {
        yield return new WaitForSeconds(1.5f);
        EnemyShouArray.Remove(target.transform);
        EnemyZhuoArray.Add(target.transform);
        target.transform.SetParent(GameObject.Find("EnemyZhuoPanel").transform);
        StartCoroutine(EnemyAttDelay());
    }

    public void BuildEnemyCard()
    {
        Debug.Log("敌人发牌？");
        if (EnemyNum <= 9)
        {
            int s = Random.Range(0, 17);
            EnemyCardObj = EnemyCardsObj[s];
            goo = Instantiate(EnemyCardObj);
            goo.tag = "EnemyCard";//Mine
            EnemyShouArray.Add(goo.transform);
            goo.transform.SetParent(GameObject.Find("EnemyShouPanel").transform);
            EnemyNum++;

            //int x = Random.Range(0, 16);
           // EnemyCardObj = EnemyCardsObj[x];
            //goo = Instantiate(EnemyCardObj, Pos.transform.position, Quaternion.identity);//生成卡牌
           // EnemyShouArray.Add(goo.transform);
            //goo.transform.SetParent(GameObject.Find("Canvas").transform);//卡牌父体为画布，否则和画布并列 不会显示出来
           // Sequence sequence = DOTween.Sequence();
           // Tweener m1 = goo.transform.DOMoveX(go.transform.position.x - 500, 1);
           // Tweener m2 = goo.transform.DOMove(new Vector3(go.transform.position.x - 1185,
           //     goo.transform.position.y + 460, 0), 1);
          //  Tweener s1 = goo.transform.DOScale(new Vector3(0.5f, 0.4f, 0), 1);
           // sequence.Append(m1);
           // sequence.AppendInterval(1);//停留一秒让玩家看一下抽的是什么牌
          //  sequence.Append(m2);
          //  sequence.Join(s1);
          //  StartCoroutine(EnemySuspend());
          //  EnemyNum++;
            //BuildCardTime = 0;
            //c++;
        }
        if (EnemyNum > 9)
        {
            Debug.Log("手牌已满！");
        }
    }



    IEnumerator EnemySuspend()
    {
        yield return new WaitForSeconds(3);
        switch (EnemyNum)
        {
            case 6:
                Vector2 z = GameObject.Find("EnemyShouPanel").GetComponent<GridLayoutGroup>().spacing;
                z.x = -205;
                GameObject.Find("EnemyShouPanel").GetComponent<GridLayoutGroup>().spacing = z;
                break;
            case 7:
                z = GameObject.Find("EnemyShouPanel").GetComponent<GridLayoutGroup>().spacing;
                z.x = -222;
                GameObject.Find("EnemyShouPanel").GetComponent<GridLayoutGroup>().spacing = z;
                break;
            case 8:
                z = GameObject.Find("EnemyShouPanel").GetComponent<GridLayoutGroup>().spacing;
                z.x = -234;
                GameObject.Find("EnemyShouPanel").GetComponent<GridLayoutGroup>().spacing = z;
                break;
            case 9:
                z = GameObject.Find("EnemyShouPanel").GetComponent<GridLayoutGroup>().spacing;
                z.x = -241;
                GameObject.Find("EnemyShouPanel").GetComponent<GridLayoutGroup>().spacing = z;
                break;
            case 10:
                z = GameObject.Find("EnemyShouPanel").GetComponent<GridLayoutGroup>().spacing;
                z.x = -248;
                GameObject.Find("EnemyShouPanel").GetComponent<GridLayoutGroup>().spacing = z;
                break;
        }
        //if (c >= 3)
        //{
            //round = Round.ToCard;
        //}
        go.transform.SetParent(GameObject.Find("EnemyShouPanel").transform);
        go.transform.GetChild(0).tag = "EnemyShouPai";
    }

    IEnumerator EnemyAttDelay()
    {
        yield return new WaitForSeconds(2.5f);//不立刻攻击 先延迟1s
        int s = Random.Range(0, EnemyZhuoArray.Count);
        m = s;
        Sequence sequence = DOTween.Sequence();
        Tweener m1 = EnemyZhuoArray[m].transform.DOMove(PlayerImg.transform.position, 0.5f);
        Tweener m2 = EnemyZhuoArray[m].transform.DOMove(EnemyZhuoArray[m].transform.position, 0.5f);
        sequence.Append(m1);
        sequence.Append(m2);
        yield return new WaitForSeconds(1);
        EnemyAtt();
    }

    public void surrender()
    {

    }

    public void chatBubleList()
    {
        
        ChatBubble1.gameObject.SetActive(true);
        ChatBubble2.gameObject.SetActive(true);
        ChatBubble3.gameObject.SetActive(true);
        ChatBubble4.gameObject.SetActive(true);
        option = true;
        
    }

    public void chat1()
    {
        
        display = true;
        ChatBubble1.gameObject.SetActive(false);
        ChatBubble2.gameObject.SetActive(false);
        ChatBubble3.gameObject.SetActive(false);
        ChatBubble4.gameObject.SetActive(false);        
        Bubble.SetActive(true);
        BubleText.text = text1.text;
        
    }

    public void chat2()
    {
        display = true;
        ChatBubble1.gameObject.SetActive(false);
        ChatBubble2.gameObject.SetActive(false);
        ChatBubble3.gameObject.SetActive(false);
        ChatBubble4.gameObject.SetActive(false);
        Bubble.SetActive(true);
        BubleText.text = text2.text;
    }

    public void chat3()
    {
        display = true;
        ChatBubble1.gameObject.SetActive(false);
        ChatBubble2.gameObject.SetActive(false);
        ChatBubble3.gameObject.SetActive(false);
        ChatBubble4.gameObject.SetActive(false);
        Bubble.SetActive(true);
        BubleText.text = text3.text;
    }

    public void chat4()
    {
        display = true;
        ChatBubble1.gameObject.SetActive(false);
        ChatBubble2.gameObject.SetActive(false);
        ChatBubble3.gameObject.SetActive(false);
        ChatBubble4.gameObject.SetActive(false);
        Bubble.SetActive(true);
        BubleText.text = text4.text;
    }

}
