using System.Collections;
using System.Collections.Generic;

namespace Data
{
    public class SkillDatabase
    {
        static SkillDatabase instace = new SkillDatabase();
        public static SkillDatabase Instance { get { return instace; } }

        #region CrowdControl
        private List<CrowdControl> ccDatabase = new List<CrowdControl>();

        private void InitializeCrowdControl()
        {
            // TAUNT = 1, GUARD = 2, REFLECT = 3 , PURITY = 4,
            // INVINCIBLE = 5, BLIND = 6, STUN = 7, DOTDAMAGE = 8,
            // ATTACKSTAT = 9, MIRRORIMAGE = 10,

            ccDatabase.Add(new CrowdControl(30101, "도발", CCTarget.SELF));
            ccDatabase.Add(new CrowdControl(30201, "방어", CCTarget.ALL));
            ccDatabase.Add(new CrowdControl(30202, "방어", CCTarget.SELF));
            ccDatabase.Add(new CrowdControl(30301, "반사", CCTarget.ALL));
            ccDatabase.Add(new CrowdControl(30401, "무효화", CCTarget.SELF));
            ccDatabase.Add(new CrowdControl(30501, "무적", CCTarget.ALL));
            ccDatabase.Add(new CrowdControl(30502, "무적", CCTarget.SELF));
            ccDatabase.Add(new CrowdControl(30601, "실명", CCTarget.ENEMY));
            ccDatabase.Add(new CrowdControl(30602, "마비", CCTarget.ENEMY));
            ccDatabase.Add(new CrowdControl(30603, "전체마비", CCTarget.ALL));
            ccDatabase.Add(new CrowdControl(30701, "스턴", CCTarget.ENEMY));
            ccDatabase.Add(new CrowdControl(30702, "전체스턴", CCTarget.ALL));
            ccDatabase.Add(new CrowdControl(30801, "출혈", CCTarget.ENEMY));
            ccDatabase.Add(new CrowdControl(30802, "중독", CCTarget.ENEMY));
            ccDatabase.Add(new CrowdControl(30803, "화상", CCTarget.ENEMY));
            ccDatabase.Add(new CrowdControl(30804, "감전", CCTarget.ENEMY));
            ccDatabase.Add(new CrowdControl(30901, "공격력 증가", CCTarget.SELF));
            ccDatabase.Add(new CrowdControl(30902, "공격력 감소", CCTarget.ENEMY));
            ccDatabase.Add(new CrowdControl(30903, "공격력 전체 증가", CCTarget.ALL));
            ccDatabase.Add(new CrowdControl(30904, "공격력 전체 감소", CCTarget.ALL));
            ccDatabase.Add(new CrowdControl(31001, "분신", CCTarget.SELF));
            ccDatabase.Add(new CrowdControl(31101, "흡혈", CCTarget.SELF));
            ccDatabase.Add(new CrowdControl(31201, "턴감소", CCTarget.SELF));
            ccDatabase.Add(new CrowdControl(31301, "혼란", CCTarget.ENEMY));
            ccDatabase.Add(new CrowdControl(31401, "상대무효화", CCTarget.ENEMY));
        }

        public CrowdControl GetCrowdControl(int id)
        {
            CrowdControl tmp = ccDatabase.Find(cc => cc.id == id);
            CrowdControl crowdControl = new CrowdControl(tmp);

            return crowdControl;
        }
        #endregion

        private static List<CharacterSkill> charSkillDB;
        private static List<MonsterSkill> monSkillDB;

        SkillDatabase()
        {
            InitializeCrowdControl();
            InitializeCharacterSkill();
            InitializeMonsterSkill();
        }

        private void InitializeCharacterSkill()
        {
            charSkillDB = new List<CharacterSkill>();
            #region Knight Skill
            charSkillDB.Add(new CharacterSkill(10100, 0, "베기", turn: 0, damage: 7, 
                description: "적을 칼로 벤다."));

            charSkillDB.Add(new CharacterSkill(10101, 1, "찌르기", turn: 0, damage: 9,
                description: "적을 칼로 강하게 찌른다."));

            charSkillDB.Add(new CharacterSkill(10102, 1, "도발", turn: 0, damage: 0,
                description: "함성을 질러 상대를 도발시킨다.", 
                new List<System.Tuple<CrowdControl, int>>()
                {
                    new System.Tuple<CrowdControl, int>(GetCrowdControl(30101), 0),
                }));

            charSkillDB.Add(new CharacterSkill(10103, 2, "작살베기", turn: 0, damage: 18
                , description: "작살로 적을 베어 큰 피해를 입힌다."));

            charSkillDB.Add(new CharacterSkill(10104, 2, "방패찍기", turn: 0, damage: 13,
                description: "방패로 상대를 내려찍는다.",
                new List<System.Tuple<CrowdControl, int>>()
                {
                    new System.Tuple<CrowdControl, int>(GetCrowdControl(30701), 1),
                }));

            charSkillDB.Add(new CharacterSkill(10105, 2, "강철방패", turn: 1, damage: 0,
                description: "강철방패로 상대의 공격을 방어한다.",
                new List<System.Tuple<CrowdControl, int>>()
                {
                    new System.Tuple<CrowdControl, int>(GetCrowdControl(30201), 0),
                }));

            charSkillDB.Add(new CharacterSkill(10106, 2, "야성의외침", turn: 0, damage: 0,
                description: "야성적인 울부짖음으로 승기를 고양시킨다.",
                new List<System.Tuple<CrowdControl, int>>()
                {
                    new System.Tuple<CrowdControl, int>(GetCrowdControl(30401), 0),
                }));

            charSkillDB.Add(new CharacterSkill(10107, 3, "반격", turn: 1, damage: 20,
                description: "상대의 공격을 방어해내고 역으로 빈틈을 공격한다.", 
                new List<int> { 10104, 10105 },
                new List<System.Tuple<CrowdControl, int>>()
                {
                    new System.Tuple<CrowdControl, int>(GetCrowdControl(30201), 0),
                    new System.Tuple<CrowdControl, int>(GetCrowdControl(30701), 1),
                }));

            charSkillDB.Add(new CharacterSkill(10108, 3, "용기", turn: 1, damage: 0,
                description: "아군의 사기를 고취시킨다.", 
                new List<int> { 10106 },
                new List<System.Tuple<CrowdControl, int>>()
                {
                    new System.Tuple<CrowdControl, int>(GetCrowdControl(30401), 0),
                    new System.Tuple<CrowdControl, int>(GetCrowdControl(30903), 0),
                }));

            charSkillDB.Add(new CharacterSkill(10109, 4, "가시방패", turn: 2, damage: 30,
                description: "가시가 돋은 방패로 상대의 공격을 방어하며 피해를 입힌다.",
                new List<int> { 10103, 10107 },
                new List<System.Tuple<CrowdControl, int>>()
                {
                    new System.Tuple<CrowdControl, int>(GetCrowdControl(30301), 0),
                }));

            charSkillDB.Add(new CharacterSkill(10110, 4, "데드락", turn: 1, damage: 30,
                description: "상대를 꼼짝 못하게 제압한다.", 
                new List<int> { 10107 },
                new List<System.Tuple<CrowdControl, int>>()
                {
                    new System.Tuple<CrowdControl, int>(GetCrowdControl(30701), 2),
                }));

            charSkillDB.Add(new CharacterSkill(10111, 4, "포효", turn: 2, damage: 10,
                description: "짐승같은 포효를 질러 아군을 보호한다.", 
                new List<int> { 10108 },
                new List<System.Tuple<CrowdControl, int>>()
                {
                    new System.Tuple<CrowdControl, int>(GetCrowdControl(30501), 0),
                }));

            #endregion

            #region Marksman Skill
            charSkillDB.Add(new CharacterSkill(10200, 0, "화살쏘기", turn: 0, damage: 9
                , description: "화살을 발사한다."));
            charSkillDB.Add(new CharacterSkill(10201, 1, "더블샷", turn: 0, damage: 11
                , description: "화살을 동시에 두 발을 발사한다."));
            charSkillDB.Add(new CharacterSkill(10202, 1, "그물화살", turn: 0, damage: 8,
                description: "그물로 몸을 옭아매는 화살을 쏜다.",
                new List<System.Tuple<CrowdControl, int>>()
                {
                    new System.Tuple<CrowdControl, int>(GetCrowdControl(30602), 1),
                }));
            charSkillDB.Add(new CharacterSkill(10203, 2, "트리플샷", turn: 0, damage: 20
                , description: "화살을 동시에 세 발을 발사한다."));
            charSkillDB.Add(new CharacterSkill(10204, 2, "톱날화살", turn: 0, damage: 16,
                description: "톱날같은 화살을 발사해 적에게 출혈을 입힌다.",
                new List<System.Tuple<CrowdControl, int>>()
                {
                    new System.Tuple<CrowdControl, int>(GetCrowdControl(30801), 3),
                }));
            charSkillDB.Add(new CharacterSkill(10205, 2, "파워샷", turn: 1, damage: 32,
                description: "기를 모아 강력한 화살을 발사한다.",
                new List<System.Tuple<CrowdControl, int>>()
                {
                    new System.Tuple<CrowdControl, int>(GetCrowdControl(30701), 1),
                }));
            charSkillDB.Add(new CharacterSkill(10206, 2, "화살촉 강화", turn: 0, damage: 0,
                description: "화살촉에 강력한 힘을 담는다.",
                new List<System.Tuple<CrowdControl, int>>()
                {
                    new System.Tuple<CrowdControl, int>(GetCrowdControl(30901), 0),
                }));
            charSkillDB.Add(new CharacterSkill(10207, 3, "화살비", turn: 1, damage: 60,
                description: "화살을 하늘에 쏘아 강력한 화살비를 적에게 내린다.",
                new List<int> { 10203 }));
            charSkillDB.Add(new CharacterSkill(10208, 3, "독화살", turn: 0, damage: 24,
                description: "독이 묻은 화살을 쏜다.", 
                new List<int> { 10204 },
                new List<System.Tuple<CrowdControl, int>>()
                {
                    new System.Tuple<CrowdControl, int>(GetCrowdControl(30802), 6),
                    new System.Tuple<CrowdControl, int>(GetCrowdControl(30602), 1),
                }));
            charSkillDB.Add(new CharacterSkill(10209, 3, "저격", turn: 1, damage: 60,
                description: "강력한 화살을 적에게 정확히 맞추어 큰 피해를 입힌다.", 
                new List<int> { 10205, 10206 },
                new List<System.Tuple<CrowdControl, int>>()
                {
                    new System.Tuple<CrowdControl, int>(GetCrowdControl(30701), 2),
                }));
            charSkillDB.Add(new CharacterSkill(10210, 3, "매 소환", turn: 0, damage: 0,
                description: "자신을 도와줄 매를 소환한다.", 
                new List<int> { 10206 },
                new List<System.Tuple<CrowdControl, int>>()
                {
                    new System.Tuple<CrowdControl, int>(GetCrowdControl(31001), 0),
                }));
            charSkillDB.Add(new CharacterSkill(10211, 4, "헤드샷", turn: 2, damage: 150,
                description: "단발에 적의 머리를 맞춰 큰 피해를 입힌다.", 
                new List<int> { 10209 },
                new List<System.Tuple<CrowdControl, int>>()
                {
                    new System.Tuple<CrowdControl, int>(GetCrowdControl(30701), 2),
                }));
            #endregion

            #region Mage Skill
            charSkillDB.Add(new CharacterSkill(10300, 0, "기본공격", turn: 0, damage: 5
                , description: "지팡이를 휘둘러 상대를 공격한다."));
            charSkillDB.Add(new CharacterSkill(10301, 1, "라이트닝",turn: 0, damage: 13
                , description: "전격을 생성하여 적에게 내리친다."));
            charSkillDB.Add(new CharacterSkill(10302, 1, "고드름", turn: 0, damage: 13
                , description: "고드름을 생성하여 발사한다."));
            charSkillDB.Add(new CharacterSkill(10303, 1, "파이어볼", turn: 0, damage: 13
                , description: "화염구를 생성하여 발사한다."));
            charSkillDB.Add(new CharacterSkill(10304, 2, "번개구름", turn: 0, damage: 20,
                description: "번개구름을 소환하여 적에게 내리친다.",
                new List<System.Tuple<CrowdControl, int>>()
                {
                    new System.Tuple<CrowdControl, int>(GetCrowdControl(30602), 1),
                }));
            charSkillDB.Add(new CharacterSkill(10305, 2, "빙결감옥", turn: 1, damage: 35,
                description: "얼음으로 된 감옥으로 상대를 가두어 얼린다.",
                new List<System.Tuple<CrowdControl, int>>()
                {
                    new System.Tuple<CrowdControl, int>(GetCrowdControl(30701), 1),
                }));
            charSkillDB.Add(new CharacterSkill(10306, 2, "블레이즈", turn: 1, damage: 48,
                description: "강력한 화염을 일으켜 상대를 불태운다.",
                new List<System.Tuple<CrowdControl, int>>()
                {
                    new System.Tuple<CrowdControl, int>(GetCrowdControl(30803), 4),
                }));
            charSkillDB.Add(new CharacterSkill(10307, 3, "전격폭발", turn: 0, damage: 30,
                description: "전격을 상대에게 터뜨려 폭발시킨다.", 
                new List<int> { 10304 },
                new List<System.Tuple<CrowdControl, int>>()
                {
                    new System.Tuple<CrowdControl, int>(GetCrowdControl(30602), 1),
                }));
            charSkillDB.Add(new CharacterSkill(10308, 3, "블리자드", turn: 1, damage: 50,
                description: "강력한 눈보라를 일으킨다.", 
                new List<int> { 10305 },
                new List<System.Tuple<CrowdControl, int>>()
                {
                    new System.Tuple<CrowdControl, int>(GetCrowdControl(30701), 1),
                }));
            charSkillDB.Add(new CharacterSkill(10309, 3, "파이어월", turn: 2, damage: 105,
                description: "거대한 불로된 벽을 일으킨다.", 
                new List<int> { 10306 },
                new List<System.Tuple<CrowdControl, int>>()
                {
                    new System.Tuple<CrowdControl, int>(GetCrowdControl(30803), 8),
                }));
            charSkillDB.Add(new CharacterSkill(10310, 4, "뇌룡", turn: 2, damage: 180,
                description: "강력한 힘을 가진 용 형상의 번개를 소환한다.", 
                new List<int> { 10307, 10309 },
                new List<System.Tuple<CrowdControl, int>>()
                {
                    new System.Tuple<CrowdControl, int>(GetCrowdControl(30602), 0),
                    new System.Tuple<CrowdControl, int>(GetCrowdControl(30804), 13),
                }));
            charSkillDB.Add(new CharacterSkill(10311, 4, "빙하기", turn: 2, damage: 180,
                description: "주변의 모든 것을 얼리는 냉기를 발산한다.", 
                new List<int> { 10307, 10308 },
                new List<System.Tuple<CrowdControl, int>>()
                {
                    new System.Tuple<CrowdControl, int>(GetCrowdControl(30701), 2),
                }));
            charSkillDB.Add(new CharacterSkill(10312, 4, "메테오", turn: 2, damage: 180,
                description: "하늘에서 수많은 운석을 소환한다.", 
                new List<int> { 10308, 10309 },
                new List<System.Tuple<CrowdControl, int>>()
                {
                    new System.Tuple<CrowdControl, int>(GetCrowdControl(30701), 1),
                    new System.Tuple<CrowdControl, int>(GetCrowdControl(30803), 11),
                }));
            #endregion
        }

        private void InitializeMonsterSkill()
        {
            monSkillDB = new List<MonsterSkill>();

            #region Dice
            monSkillDB.Add(new MonsterSkill(200110, "대기", 0, -1,
                description: "아무런 행동도 취하지 않는다."));

            #region 미노타
            monSkillDB.Add(new MonsterSkill(201111, "바위 던지기", 0, cost: 2,
                description: "잘 깨지는 바위를 던져 상대를 상처입힌다.",
                new List<System.Tuple<CrowdControl, int>>()
                {
                    new System.Tuple<CrowdControl, int>(GetCrowdControl(30602), 2),
                }));
            monSkillDB.Add(new MonsterSkill(201112, "위협적인 눈빛", 0, cost: 1,
                description: "상대 1명을 위협적으로 노려봐 사기를 저하시킨다.",
                new List<System.Tuple<CrowdControl, int>>()
                {
                    new System.Tuple<CrowdControl, int>(GetCrowdControl(30902), 0),
                }));
            monSkillDB.Add(new MonsterSkill(201113, "소리지르기", 0, cost: 1,
                description: "고함소리를 질러 상대의 집중을 흐트린다.",
                new List<System.Tuple<CrowdControl, int>>()
                {
                    new System.Tuple<CrowdControl, int>(GetCrowdControl(31401), 0),
                }));
            monSkillDB.Add(new MonsterSkill(201121, "돌진", 0, cost: 4,
                description: "강력한 박치기로 1명을 행동 불능으로 만든다.",
                new List<System.Tuple<CrowdControl, int>>()
                {
                    new System.Tuple<CrowdControl, int>(GetCrowdControl(30701), 2),
                }));
            monSkillDB.Add(new MonsterSkill(201122, "단단해지기", 0, cost: 3,
                description: "근육량을 증가시켜 방어력을 상승시킨다.",
                new List<System.Tuple<CrowdControl, int>>()
                {
                    new System.Tuple<CrowdControl, int>(GetCrowdControl(30904), 0),
                }));
            monSkillDB.Add(new MonsterSkill(201123, "밥상뒤엎기", 0, cost: 3,
                description: "자기 앞의 지면을 뒤집어 적의 해로운 효과를 방어한다.",
                new List<System.Tuple<CrowdControl, int>>()
                {
                    new System.Tuple<CrowdControl, int>(GetCrowdControl(30202), 0),
                }));
            monSkillDB.Add(new MonsterSkill(201131, "지진", 0, cost: 7,
                description: "큰 지진을 이르켜 상대 전체를 행동 불능으로 만든다.",
                new List<System.Tuple<CrowdControl, int>>()
                {
                    new System.Tuple<CrowdControl, int>(GetCrowdControl(30702), 2),
                }));
            monSkillDB.Add(new MonsterSkill(201132, "성난황소", 0, cost: 5,
                description: "미노타우르스가 분노하여 해로운 효과에 면역을 갖는다.",
                new List<System.Tuple<CrowdControl, int>>()
                {
                    new System.Tuple<CrowdControl, int>(GetCrowdControl(30502), 0),
                }));
            #endregion

            #region 스네이크
            monSkillDB.Add(new MonsterSkill(203111, "물기", 0, cost: 2,
                description: "상대를 물어 마비독을 주입한다.",
                new List<System.Tuple<CrowdControl, int>>()
                {
                    new System.Tuple<CrowdControl, int>(GetCrowdControl(30602), 2),
                }));
            monSkillDB.Add(new MonsterSkill(203112, "탈피", 0, cost: 1,
                description: "탈피하여 안 좋은 효과들을 해제한다.",
                new List<System.Tuple<CrowdControl, int>>()
                {
                    new System.Tuple<CrowdControl, int>(GetCrowdControl(30401), 0),
                }));
            monSkillDB.Add(new MonsterSkill(203113, "뱀의 눈", 0, cost: 1,
                description: "뱀 눈으로 상대를 위협해 사기를 떨어뜨린다.",
                new List<System.Tuple<CrowdControl, int>>()
                {
                    new System.Tuple<CrowdControl, int>(GetCrowdControl(30902), 0),
                }));
            monSkillDB.Add(new MonsterSkill(203121, "꼬리속박", 0, cost: 5,
                description: "꼬리로 상대 1명을 행동 불능으로 만든다.",
                new List<System.Tuple<CrowdControl, int>>()
                {
                    new System.Tuple<CrowdControl, int>(GetCrowdControl(30701), 1),
                }));            
            monSkillDB.Add(new MonsterSkill(203122, "또아리말기", 0, cost: 3,
                description: "몸을 말아 상대 공격을 방어한다.",
                new List<System.Tuple<CrowdControl, int>>()
                {
                    new System.Tuple<CrowdControl, int>(GetCrowdControl(30202), 0),
                }));
            monSkillDB.Add(new MonsterSkill(203123, "흡혈", 0, cost: 2,
                description: "상대 체액을 빨아 체력을 흡수한다.",
                new List<System.Tuple<CrowdControl, int>>()
                {
                    new System.Tuple<CrowdControl, int>(GetCrowdControl(31101), 25),
                }));            
            monSkillDB.Add(new MonsterSkill(203131, "독안개", 0, cost: 6,
                description: "마비독 안개를 생성하여 모든 적을 마비시킨다.",
                new List<System.Tuple<CrowdControl, int>>()
                {
                    new System.Tuple<CrowdControl, int>(GetCrowdControl(30603), 2),
                }));
            monSkillDB.Add(new MonsterSkill(203132, "선악과", 0, cost: 7,
                description: "상대를 유혹하여 선악과를 먹게 하여 혼란에 빠뜨린다.",
                new List<System.Tuple<CrowdControl, int>>()
                {
                    new System.Tuple<CrowdControl, int>(GetCrowdControl(31301), 0),
                }));
            #endregion

            #region 구미호
            monSkillDB.Add(new MonsterSkill(204111, "회피", 0, cost: 2,
                description: "빠르게 움직여 상대의 특수효과를 피한다.",
                new List<System.Tuple<CrowdControl, int>>()
                {
                    new System.Tuple<CrowdControl, int>(GetCrowdControl(30202), 0),
                }));
            monSkillDB.Add(new MonsterSkill(204112, "정기 흡수", 0, cost: 1,
                description: "상대의 정기를 흡수하여 이로운 효과를 제거한다.",
                new List<System.Tuple<CrowdControl, int>>()
                {
                    new System.Tuple<CrowdControl, int>(GetCrowdControl(31401), 0),
                }));
            monSkillDB.Add(new MonsterSkill(204113, "여우불", 0, cost: 0,
                description: "여우불을 소환하여 상대에게 더 큰 효과를 준다.",
                new List<System.Tuple<CrowdControl, int>>()
                {
                    new System.Tuple<CrowdControl, int>(GetCrowdControl(31001), 0),
                }));
            monSkillDB.Add(new MonsterSkill(204114, "매혹적인 눈빛", 0, cost: 1,
                description: "매혹적인 눈빛으로 상대의 공격 의지를 감소시킨다.",
                new List<System.Tuple<CrowdControl, int>>()
                {
                    new System.Tuple<CrowdControl, int>(GetCrowdControl(30902), 0),
                }));
            monSkillDB.Add(new MonsterSkill(204121, "털갈이", 0, cost: 2,
                description: "오염된 털을 제거한다.",
                new List<System.Tuple<CrowdControl, int>>()
                {
                    new System.Tuple<CrowdControl, int>(GetCrowdControl(30401), 0),
                }));
            monSkillDB.Add(new MonsterSkill(204131, "유혹", 0, cost: 5,
                description: "상대를 유혹하여 혼란에 빠뜨린다.",
                new List<System.Tuple<CrowdControl, int>>()
                {
                    new System.Tuple<CrowdControl, int>(GetCrowdControl(31301), 0),
                }));
            monSkillDB.Add(new MonsterSkill(204132, "여우구슬", 0, cost: 5,
                description: "여우구슬을 통해 기운을 모은다.",
                new List<System.Tuple<CrowdControl, int>>()
                {
                    new System.Tuple<CrowdControl, int>(GetCrowdControl(31201), 0),
                    new System.Tuple<CrowdControl, int>(GetCrowdControl(31101), 20),
                }));
            monSkillDB.Add(new MonsterSkill(204133, "정신조종", 0, cost: 5,
                description: "정신을 조종하여 자신을 공격하게 만든다.",
                new List<System.Tuple<CrowdControl, int>>()
                {
                    new System.Tuple<CrowdControl, int>(GetCrowdControl(30301), 0),
                }));
            #endregion
            #endregion

            #region Attack
            monSkillDB.Add(new MonsterSkill(200211, "전체공격", 10, cost: 0,
                description: "모든 적을 공격한다."));
            monSkillDB.Add(new MonsterSkill(200311, "단일공격", 4, cost: 0,
                description: "하나의 적을 공격한다."));
            monSkillDB.Add(new MonsterSkill(200411, "스턴공격", 5, cost: 0,
                description: "하나의 적을 공격한 뒤 다른 하나의 적을 기절에 걸리게 한다.",
                new List<System.Tuple<CrowdControl, int>>()
                {
                    new System.Tuple<CrowdControl, int>(GetCrowdControl(30701), 2),
                }));
            #endregion


        }

        public CharacterSkill GetCharacterSkill(int id)
        {
            return charSkillDB.Find(skill => skill.id == id);
        }

        public MonsterSkill GetMonsterSkill(int id)
        {
            return monSkillDB.Find(skill => skill.id == id);
        }

        public CharacterSkill GetCharacterBasicSkill(int charId)
        {
            return charSkillDB.Find(skill =>
            (skill.id / 100 == (charId % 100 + 100))
            && (skill.id % 10 == 0));
        }

        public MonsterSkill GetMonsterBasicSkill(int monId)
        {
            return monSkillDB.Find(skill =>
            (skill.id / 1000 == (monId))
            && (skill.id % 10 == 0));
        }

        public List<CharacterSkill> GetCharacterAllSkills(int charNumber)
        {
            // charNumber 
            // 1: Knight, 2: Marksman, 3: Mage
            charNumber += 100;
            return charSkillDB.FindAll(skill => skill.id/100 == charNumber);
        }

        public List<int> GetMonsterAllSkills(int monsterNumber)
        {
            // charNumber 
            // 201 Mino, 203 Snake 204 Ninetail

            List<MonsterSkill> skills = monSkillDB.FindAll(skill => skill.id / 1000 == monsterNumber);

            List<int> dices = new List<int>();
            dices.Add(200110);
            foreach (MonsterSkill skill in skills) dices.Add(skill.id);

            return dices;
        }

        public List<CharacterSkill> GetCharacterDices(int id)
        {
            Character character = CharacterDatabase.Instance.GetCharacter(id);

            if(character == null) return null;

            List<CharacterSkill> characterSkills = new List<CharacterSkill>();
            foreach (CharacterSkill skill in character.mySkills)
            {
                characterSkills.Add(skill);
            }

            return characterSkills;
        }
        public List<MonsterSkill> GetMonsterDices(int id)
        {
            Monster monster = MonsterDatabase.Instance.GetMonster(id);

            if (monster == null) return null;

            List<MonsterSkill> monsterSkills = new List<MonsterSkill>();
            foreach (int skillId in monster.diceSkills)
            {
                monsterSkills.Add(GetMonsterSkill(skillId));
            }

            return monsterSkills;
        }

        public List<MonsterSkill> GetMonsterAttackSkills(int id, int round)
        {
            Monster monster = MonsterDatabase.Instance.GetMonster(id);

            if (monster == null) return null;

            List<MonsterSkill> monsterSkills = new List<MonsterSkill>();
            foreach (int skillId in monster.attackSkills)
            {
                MonsterSkill skill = GetMonsterSkill(skillId);
                if (skill.tier <= round)
                    monsterSkills.Add(skill);

                if (monsterSkills.Count >= 3) break;
            }

            return monsterSkills;
        }
    }
}
