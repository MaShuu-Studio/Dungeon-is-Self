using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using Data;

public class SkillDescription : UIDescription
{
    [SerializeField] private Image skillImage;
    [SerializeField] private Text nameText;
    [SerializeField] private Text typeText;
    [SerializeField] private Text dmgText;
    [SerializeField] private Text descriptionText;
    [SerializeField] private Transform ccListTransform;
    [SerializeField] private GameObject ccPrefab;
    [SerializeField] private Text readyTurnText;

    private List<CrowdControlIcon> ccIcons = new List<CrowdControlIcon>();
    private int skillId = -1;
    public int SkillID { get { return skillId; } }

    protected override void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        skillImage.color = Color.black;
        nameText.text = "";
        typeText.text = "";
        descriptionText.text = "";
        dmgText.text = "";
        readyTurnText.text = "";
    }
    public void SetDescription(int id, string name, string type, string description, string dmg, Dictionary<CrowdControl, int> ccs = null, int readyTurn = -1)
    {
        skillId = id;
        skillImage.color = Color.white;
        skillImage.sprite = Resources.Load<Sprite>("Sprites/Skills/" + id.ToString());
        nameText.text = name;
        typeText.text = type;
        descriptionText.text = description;
        dmgText.text = dmg;
        
        foreach (CrowdControlIcon ccIcon in ccIcons) Destroy(ccIcon.gameObject);
        ccIcons.Clear();
        if (ccs != null)
        {
            foreach (CrowdControl cc in ccs.Keys)
            {
                GameObject obj = Instantiate(ccPrefab);
                obj.transform.SetParent(ccListTransform);
                CrowdControlIcon ccIcon = obj.GetComponent<CrowdControlIcon>();
                ccIcon.SetImage(cc.id);
                int turn = cc.turn - 1;
                if (ccs[cc] != 0) turn = -1;
                ccIcon.SetTurn(turn, ccs[cc]);
                ccIcons.Add(ccIcon);
            }
        }

        if (readyTurn == -1) readyTurnText.text = "";
        else readyTurnText.text = "WAIT TURN " + readyTurn;
    }

    public void ResetDescription()
    {

    }
}
