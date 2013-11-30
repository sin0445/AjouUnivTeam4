using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;

namespace KinectEducationForKids
{
    public static class CharacterListLibrary
    {
        public enum CHARACTERTYPE
        {
            CONSONANT = 1,
            VOWEL = 2
        };
        public static List<CharacterBase> getCharacters(CHARACTERTYPE type)
        {
            List<CharacterBase> charList;
            switch (type)
            {
                case CHARACTERTYPE.CONSONANT:
                    charList = getConsonants();
                    break;
                case CHARACTERTYPE.VOWEL:
                    charList = getVowels();
                    break;
                default:
                    charList = null;
                    break;
            }
            return charList;
        }
        private static List<CharacterBase> getConsonants()
        {
            List<CharacterBase> consList = new List<CharacterBase>();
            //여기다 기역부터 히읗까지 아래와 같은 방식으로 전부 추가 시키면 된다. 
            consList.Add(new Kiyeok());
            consList.Add(new Nieun());
            consList.Add(new Digeuk());
            consList.Add(new Rieul());
            consList.Add(new Mium());
            consList.Add(new Bium());
            consList.Add(new Siot());
            consList.Add(new Ieung());
            consList.Add(new Jioet());
            consList.Add(new Chioet());
            consList.Add(new Kioek());
            consList.Add(new Tioet());
            consList.Add(new Peoup());
            consList.Add(new Hieong());
            return consList;
        }

        private static List<CharacterBase> getVowels()
        {
            List<CharacterBase> vowList = new List<CharacterBase>();
            //여기다 ㅏ 부터 ㅣ 까지 클래스를 정의한 후 위의 디귿과 같은 방식으로 추가시키면 된다.
            vowList.Add(new Ah());
            vowList.Add(new Yah());
            vowList.Add(new Erh());
            vowList.Add(new Yerh());
            vowList.Add(new Oh());
            vowList.Add(new Yoh());
            vowList.Add(new Ooh());
            vowList.Add(new Yuh());
            vowList.Add(new Uh());
            vowList.Add(new Eh());
            return vowList;
        }
    }

    public abstract class CharacterBase
    {
        public String CharacterName;
        public List<Point> DotList;
        public List<List<int>> StrokeDotIndex;

        public abstract void setDotList();
        public abstract void setStrokeDotIndex();
        public abstract string getPath();
    }

    class Kiyeok : CharacterBase
    {
        public Kiyeok()
        {
            this.DotList = new List<Point>();
            this.StrokeDotIndex = new List<List<int>>();
            this.CharacterName = "기역";
            setDotList();
            setStrokeDotIndex();
        }
        public override void setDotList()
        {
            this.DotList.Add(new Point(100, 250));
            this.DotList.Add(new Point(200, 250));
            this.DotList.Add(new Point(300, 250));
            this.DotList.Add(new Point(400, 250));
            this.DotList.Add(new Point(500, 250));
            this.DotList.Add(new Point(600, 250));
            this.DotList.Add(new Point(700, 250));
            this.DotList.Add(new Point(700, 350));
            this.DotList.Add(new Point(700, 450));
            this.DotList.Add(new Point(700, 550));
        }
        public override void setStrokeDotIndex()
        {
            List<int> Stroke = new List<int>();

            Stroke.Add(0);
            Stroke.Add(1);
            Stroke.Add(2);
            Stroke.Add(3);
            Stroke.Add(4);
            Stroke.Add(5);
            Stroke.Add(6);
            Stroke.Add(7);
            Stroke.Add(8);
            Stroke.Add(9);
            this.StrokeDotIndex.Add(Stroke);
        }
        public override string getPath()
        {
            return "con_1.png";
        }
    }

    public class Nieun : CharacterBase
    {
        public Nieun()
        {
            this.DotList = new List<Point>();
            this.StrokeDotIndex = new List<List<int>>();
            this.CharacterName = "니은";
            setDotList();
            setStrokeDotIndex();
        }

        public override void setDotList()
        {
            this.DotList.Add(new Point(100, 250));
            this.DotList.Add(new Point(100, 350));
            this.DotList.Add(new Point(100, 450));
            this.DotList.Add(new Point(100, 550));
            this.DotList.Add(new Point(200, 550));
            this.DotList.Add(new Point(300, 550));
            this.DotList.Add(new Point(400, 550));
            this.DotList.Add(new Point(500, 550));
            this.DotList.Add(new Point(600, 550));
            this.DotList.Add(new Point(700, 550));
        }
        public override void setStrokeDotIndex()
        {
            List<int> Stroke = new List<int>();

            Stroke.Add(0);
            Stroke.Add(1);
            Stroke.Add(2);
            Stroke.Add(3);
            Stroke.Add(4);
            Stroke.Add(5);
            Stroke.Add(6);
            Stroke.Add(7);
            Stroke.Add(8);
            Stroke.Add(9);
            this.StrokeDotIndex.Add(Stroke);
        }
        public override string getPath()
        {
            return "con_2.png";
        }
    }

    public class Digeuk : CharacterBase
    {
        public Digeuk()
        {
            this.DotList = new List<Point>();
            this.StrokeDotIndex = new List<List<int>>();
            this.CharacterName = "디귿";

            setDotList();
            setStrokeDotIndex();
        }
        public override void setDotList()
        {
            this.DotList.Add(new Point(100, 250));
            this.DotList.Add(new Point(250, 250));
            this.DotList.Add(new Point(400, 250));
            this.DotList.Add(new Point(550, 250));
            this.DotList.Add(new Point(700, 250));
            this.DotList.Add(new Point(100, 350));
            this.DotList.Add(new Point(100, 450));
            this.DotList.Add(new Point(100, 550));
            this.DotList.Add(new Point(250, 550));
            this.DotList.Add(new Point(400, 550));
            this.DotList.Add(new Point(550, 550));
            this.DotList.Add(new Point(700, 550));
        }
        public override void setStrokeDotIndex()
        {
            List<int> Stroke = new List<int>();

            Stroke.Add(0);
            Stroke.Add(1);
            Stroke.Add(2);
            Stroke.Add(3);
            Stroke.Add(4);
            this.StrokeDotIndex.Add(Stroke);

            Stroke = new List<int>();

            Stroke.Add(0);
            Stroke.Add(5);
            Stroke.Add(6);
            Stroke.Add(7);
            Stroke.Add(8);
            Stroke.Add(9);
            Stroke.Add(10);
            Stroke.Add(11);
            this.StrokeDotIndex.Add(Stroke);
        }

        public override string getPath()
        {
            return "con_3.png";
        }
    }

    public class Rieul : CharacterBase
    {
        public Rieul()
        {
            this.DotList = new List<Point>();
            this.StrokeDotIndex = new List<List<int>>();
            this.CharacterName = "리을";
            setDotList();
            setStrokeDotIndex();
        }
        public override void setDotList()
        {
            this.DotList.Add(new Point(100, 220));
            this.DotList.Add(new Point(250, 220));
            this.DotList.Add(new Point(400, 220));
            this.DotList.Add(new Point(550, 220));
            this.DotList.Add(new Point(700, 220));
            this.DotList.Add(new Point(700, 310));
            this.DotList.Add(new Point(700, 400));

            this.DotList.Add(new Point(100, 400));
            this.DotList.Add(new Point(250, 400));
            this.DotList.Add(new Point(400, 400));
            this.DotList.Add(new Point(550, 400));

            this.DotList.Add(new Point(100, 490));
            this.DotList.Add(new Point(100, 580));
            this.DotList.Add(new Point(250, 580));
            this.DotList.Add(new Point(400, 580));
            this.DotList.Add(new Point(550, 580));
            this.DotList.Add(new Point(700, 580));
        }
        public override void setStrokeDotIndex()
        {
            List<int> Stroke = new List<int>();

            Stroke.Add(0);
            Stroke.Add(1);
            Stroke.Add(2);
            Stroke.Add(3);
            Stroke.Add(4);
            Stroke.Add(5);
            Stroke.Add(6);
            this.StrokeDotIndex.Add(Stroke);

            Stroke = new List<int>();
            Stroke.Add(7);
            Stroke.Add(8);
            Stroke.Add(9);
            Stroke.Add(10);
            Stroke.Add(6);
            this.StrokeDotIndex.Add(Stroke);

            Stroke = new List<int>();

            Stroke.Add(7);
            Stroke.Add(11);
            Stroke.Add(12);
            Stroke.Add(13);
            Stroke.Add(14);
            Stroke.Add(15);
            Stroke.Add(16);
            this.StrokeDotIndex.Add(Stroke);
        }

        public override string getPath()
        {
            return "con_4.png";
        }
    }

    public class Mium : CharacterBase
    {
        public Mium()
        {
            this.DotList = new List<Point>();
            this.StrokeDotIndex = new List<List<int>>();
            this.CharacterName = "미음";
            setDotList();
            setStrokeDotIndex();
        }
        public override void setDotList()
        {
            this.DotList.Add(new Point(100, 220));
            this.DotList.Add(new Point(100, 340));
            this.DotList.Add(new Point(100, 460));
            this.DotList.Add(new Point(100, 580));

            this.DotList.Add(new Point(250, 220));
            this.DotList.Add(new Point(400, 220));
            this.DotList.Add(new Point(550, 220));
            this.DotList.Add(new Point(700, 220));
            this.DotList.Add(new Point(700, 340));
            this.DotList.Add(new Point(700, 460));
            this.DotList.Add(new Point(700, 580));

            this.DotList.Add(new Point(100, 580));
            this.DotList.Add(new Point(250, 580));
            this.DotList.Add(new Point(400, 580));
            this.DotList.Add(new Point(550, 580));
        }
        public override void setStrokeDotIndex()
        {
            List<int> Stroke = new List<int>();

            Stroke.Add(0);
            Stroke.Add(1);
            Stroke.Add(2);
            Stroke.Add(3);
            this.StrokeDotIndex.Add(Stroke);

            Stroke = new List<int>();
            Stroke.Add(0);
            Stroke.Add(4);
            Stroke.Add(5);
            Stroke.Add(6);
            Stroke.Add(7);
            Stroke.Add(8);
            Stroke.Add(9);
            Stroke.Add(10);
            this.StrokeDotIndex.Add(Stroke);

            Stroke = new List<int>();
            Stroke.Add(3);
            Stroke.Add(11);
            Stroke.Add(12);
            Stroke.Add(13);
            Stroke.Add(14);
            Stroke.Add(10);
            this.StrokeDotIndex.Add(Stroke);

        }

        public override string getPath()
        {
            return "con_5.png";
        }
    }

    public class Bium : CharacterBase
    {
        public Bium()
        {
            this.DotList = new List<Point>();
            this.StrokeDotIndex = new List<List<int>>();
            this.CharacterName = "비읍";
            setDotList();
            setStrokeDotIndex();
        }
        public override void setDotList()
        {
            this.DotList.Add(new Point(100, 220));
            this.DotList.Add(new Point(100, 340));
            this.DotList.Add(new Point(100, 460));
            this.DotList.Add(new Point(100, 580));

            this.DotList.Add(new Point(700, 220));
            this.DotList.Add(new Point(700, 340));
            this.DotList.Add(new Point(700, 460));
            this.DotList.Add(new Point(700, 580));

            this.DotList.Add(new Point(100, 400));
            this.DotList.Add(new Point(250, 400));
            this.DotList.Add(new Point(400, 400));
            this.DotList.Add(new Point(550, 400));
            this.DotList.Add(new Point(700, 400));

            this.DotList.Add(new Point(100, 580));
            this.DotList.Add(new Point(250, 580));
            this.DotList.Add(new Point(400, 580));
            this.DotList.Add(new Point(550, 580));
        }
        public override void setStrokeDotIndex()
        {
            List<int> Stroke = new List<int>();

            Stroke.Add(0);
            Stroke.Add(1);
            Stroke.Add(2);
            Stroke.Add(3);
            this.StrokeDotIndex.Add(Stroke);

            Stroke = new List<int>();
            Stroke.Add(4);
            Stroke.Add(5);
            Stroke.Add(6);
            Stroke.Add(7);
            this.StrokeDotIndex.Add(Stroke);

            Stroke = new List<int>();
            Stroke.Add(8);
            Stroke.Add(9);
            Stroke.Add(10);
            Stroke.Add(11);
            Stroke.Add(12);
            this.StrokeDotIndex.Add(Stroke);

            Stroke = new List<int>();
            Stroke.Add(3);
            Stroke.Add(13);
            Stroke.Add(14);
            Stroke.Add(15);
            Stroke.Add(16);
            Stroke.Add(7);
            this.StrokeDotIndex.Add(Stroke);
        }

        public override string getPath()
        {
            return "con_6.png";
        }
    }

    public class Siot : CharacterBase
    {
        public Siot()
        {
            this.DotList = new List<Point>();
            this.StrokeDotIndex = new List<List<int>>();
            this.CharacterName = "시옷";

            setDotList();
            setStrokeDotIndex();
        }
        public override void setDotList()
        {
            this.DotList.Add(new Point(420, 150));
            this.DotList.Add(new Point(340, 250));
            this.DotList.Add(new Point(260, 350));
            this.DotList.Add(new Point(180, 450));
            this.DotList.Add(new Point(100, 550));

            this.DotList.Add(new Point(420, 350));
            this.DotList.Add(new Point(500, 450));
            this.DotList.Add(new Point(580, 550));
        }
        public override void setStrokeDotIndex()
        {
            List<int> Stroke = new List<int>();

            Stroke.Add(0);
            Stroke.Add(1);
            Stroke.Add(2);
            Stroke.Add(3);
            Stroke.Add(4);
            this.StrokeDotIndex.Add(Stroke);

            Stroke = new List<int>();
            Stroke.Add(1);
            Stroke.Add(5);
            Stroke.Add(6);
            Stroke.Add(7);
            this.StrokeDotIndex.Add(Stroke);
        }

        public override string getPath()
        {
            return "con_7.png";
        }
    }

    public class Ieung : CharacterBase
    {
        public Ieung()
        {
            this.DotList = new List<Point>();
            this.StrokeDotIndex = new List<List<int>>();
            this.CharacterName = "이응";
            setDotList();
            setStrokeDotIndex();
        }
        public override void setDotList()
        {
            this.DotList.Add(new Point(375, 175));
            this.DotList.Add(new Point(400, 200));
            this.DotList.Add(new Point(300, 228));
            this.DotList.Add(new Point(228, 300));
            this.DotList.Add(new Point(200, 400));
            this.DotList.Add(new Point(228, 500));
            this.DotList.Add(new Point(300, 572));
            this.DotList.Add(new Point(400, 600));
            this.DotList.Add(new Point(500, 572));
            this.DotList.Add(new Point(572, 500));
            this.DotList.Add(new Point(600, 400));
            this.DotList.Add(new Point(572, 300));
            this.DotList.Add(new Point(500, 228));
        }
        public override void setStrokeDotIndex()
        {
            List<int> Stroke = new List<int>();

            Stroke.Add(0);
            Stroke.Add(1);
            Stroke.Add(2);
            Stroke.Add(3);
            Stroke.Add(4);
            Stroke.Add(5);
            Stroke.Add(6);
            Stroke.Add(7);
            Stroke.Add(8);
            Stroke.Add(9);
            Stroke.Add(10);
            Stroke.Add(11);
            Stroke.Add(12);
            Stroke.Add(1);
            this.StrokeDotIndex.Add(Stroke);
        }

        public override string getPath()
        {
            return "con_8.png";
        }
    }

    public class Jioet : CharacterBase
    {
        public Jioet()
        {
            this.DotList = new List<Point>();
            this.StrokeDotIndex = new List<List<int>>();
            this.CharacterName = "지읒";
            setDotList();
            setStrokeDotIndex();
        }
        public override void setDotList()
        {
            this.DotList.Add(new Point(200, 220));
            this.DotList.Add(new Point(330, 220));
            this.DotList.Add(new Point(466, 220));
            this.DotList.Add(new Point(599, 220));
            this.DotList.Add(new Point(580, 290));
            this.DotList.Add(new Point(460, 360));
            this.DotList.Add(new Point(340, 430));
            this.DotList.Add(new Point(220, 500));
            this.DotList.Add(new Point(100, 570));

            this.DotList.Add(new Point(400, 400));
            this.DotList.Add(new Point(550, 485));
            this.DotList.Add(new Point(700, 570));
        }
        public override void setStrokeDotIndex()
        {
            List<int> Stroke = new List<int>();

            Stroke.Add(0);
            Stroke.Add(1);
            Stroke.Add(2);
            Stroke.Add(3);
            Stroke.Add(4);
            Stroke.Add(5);
            Stroke.Add(6);
            Stroke.Add(7);
            Stroke.Add(8);
            this.StrokeDotIndex.Add(Stroke);

            Stroke = new List<int>();
            Stroke.Add(9);
            Stroke.Add(10);
            Stroke.Add(11);
            this.StrokeDotIndex.Add(Stroke);
        }

        public override string getPath()
        {
            return "con_9.png";
        }
    }

    public class Chioet : CharacterBase
    {
        public Chioet()
        {
            this.DotList = new List<Point>();
            this.StrokeDotIndex = new List<List<int>>();
            this.CharacterName = "치읓";
            setDotList();
            setStrokeDotIndex();
        }
        public override void setDotList()
        {
            this.DotList.Add(new Point(260, 200));
            this.DotList.Add(new Point(380, 200));
            this.DotList.Add(new Point(500, 200));

            this.DotList.Add(new Point(200, 290));
            this.DotList.Add(new Point(320, 290));
            this.DotList.Add(new Point(440, 290));
            this.DotList.Add(new Point(560, 290));
            this.DotList.Add(new Point(460, 360));
            this.DotList.Add(new Point(340, 430));
            this.DotList.Add(new Point(220, 500));
            this.DotList.Add(new Point(100, 570));

            this.DotList.Add(new Point(400, 400));
            this.DotList.Add(new Point(550, 485));
            this.DotList.Add(new Point(700, 570));
        }
        public override void setStrokeDotIndex()
        {
            List<int> Stroke = new List<int>();

            Stroke.Add(0);
            Stroke.Add(1);
            Stroke.Add(2);
            this.StrokeDotIndex.Add(Stroke);

            Stroke = new List<int>();

            Stroke.Add(3);
            Stroke.Add(4);
            Stroke.Add(5);
            Stroke.Add(6);
            Stroke.Add(7);
            Stroke.Add(8);
            Stroke.Add(9);
            Stroke.Add(10);
            this.StrokeDotIndex.Add(Stroke);

            Stroke = new List<int>();
            Stroke.Add(11);
            Stroke.Add(12);
            Stroke.Add(13);
            this.StrokeDotIndex.Add(Stroke);

        }

        public override string getPath()
        {
            return "con_10.png";
        }
    }

    public class Kioek : CharacterBase
    {
        public Kioek()
        {
            this.DotList = new List<Point>();
            this.StrokeDotIndex = new List<List<int>>();
            this.CharacterName = "키읔";
            setDotList();
            setStrokeDotIndex();
        }
        public override void setDotList()
        {
            this.DotList.Add(new Point(300, 250));
            this.DotList.Add(new Point(400, 250));
            this.DotList.Add(new Point(500, 250));
            this.DotList.Add(new Point(600, 250));
            this.DotList.Add(new Point(700, 250));

            this.DotList.Add(new Point(675, 320));
            this.DotList.Add(new Point(630, 390));
            this.DotList.Add(new Point(580, 460));
            this.DotList.Add(new Point(540, 530));
            this.DotList.Add(new Point(500, 600));

            this.DotList.Add(new Point(370, 390));
            this.DotList.Add(new Point(500, 390));

        }
        public override void setStrokeDotIndex()
        {
            List<int> Stroke = new List<int>();

            Stroke.Add(0);
            Stroke.Add(1);
            Stroke.Add(2);
            Stroke.Add(3);
            Stroke.Add(4);
            Stroke.Add(5);
            Stroke.Add(6);
            Stroke.Add(7);
            Stroke.Add(8);
            Stroke.Add(9);
            this.StrokeDotIndex.Add(Stroke);

            Stroke = new List<int>();
            Stroke.Add(10);
            Stroke.Add(11);
            Stroke.Add(6);
            this.StrokeDotIndex.Add(Stroke);
        }

        public override string getPath()
        {
            return "con_11.png";
        }
    }

    public class Tioet : CharacterBase
    {
        public Tioet()
        {
            this.DotList = new List<Point>();
            this.StrokeDotIndex = new List<List<int>>();
            this.CharacterName = "티읕";
            setDotList();
            setStrokeDotIndex();
        }
        public override void setDotList()
        {
            this.DotList.Add(new Point(100, 200));
            this.DotList.Add(new Point(250, 200));
            this.DotList.Add(new Point(400, 200));
            this.DotList.Add(new Point(550, 200));
            this.DotList.Add(new Point(700, 200));


            this.DotList.Add(new Point(100, 400));
            this.DotList.Add(new Point(200, 400));
            this.DotList.Add(new Point(350, 400));
            this.DotList.Add(new Point(500, 400));
            this.DotList.Add(new Point(650, 400));

            this.DotList.Add(new Point(100, 300));
            this.DotList.Add(new Point(100, 500));
            this.DotList.Add(new Point(100, 600));
            this.DotList.Add(new Point(250, 600));
            this.DotList.Add(new Point(400, 600));
            this.DotList.Add(new Point(550, 600));
            this.DotList.Add(new Point(700, 600));


        }
        public override void setStrokeDotIndex()
        {
            List<int> Stroke = new List<int>();

            Stroke.Add(0);
            Stroke.Add(1);
            Stroke.Add(2);
            Stroke.Add(3);
            Stroke.Add(4);
            this.StrokeDotIndex.Add(Stroke);

            Stroke = new List<int>();
            Stroke.Add(5);
            Stroke.Add(6);
            Stroke.Add(7);
            Stroke.Add(8);
            Stroke.Add(9);
            this.StrokeDotIndex.Add(Stroke);

            Stroke = new List<int>();
            Stroke.Add(0);
            Stroke.Add(10);
            Stroke.Add(5);
            Stroke.Add(11);
            Stroke.Add(12);
            Stroke.Add(13);
            Stroke.Add(14);
            Stroke.Add(15);
            Stroke.Add(16);
            this.StrokeDotIndex.Add(Stroke);
        }

        public override string getPath()
        {
            return "con_12.png";
        }
    }

    public class Peoup : CharacterBase
    {
        public Peoup()
        {
            this.DotList = new List<Point>();
            this.StrokeDotIndex = new List<List<int>>();
            this.CharacterName = "피읖";
            setDotList();
            setStrokeDotIndex();
        }
        public override void setDotList()
        {
            this.DotList.Add(new Point(150, 250));
            this.DotList.Add(new Point(280, 250));
            this.DotList.Add(new Point(410, 250));
            this.DotList.Add(new Point(540, 250));
            this.DotList.Add(new Point(670, 250));

            this.DotList.Add(new Point(307, 400));
            this.DotList.Add(new Point(333, 550));

            this.DotList.Add(new Point(513, 400));
            this.DotList.Add(new Point(487, 550));


            this.DotList.Add(new Point(100, 550));
            this.DotList.Add(new Point(255, 550));
            this.DotList.Add(new Point(410, 550));
            this.DotList.Add(new Point(565, 550));
            this.DotList.Add(new Point(720, 550));
        }
        public override void setStrokeDotIndex()
        {
            List<int> Stroke = new List<int>();

            Stroke.Add(0);
            Stroke.Add(1);
            Stroke.Add(2);
            Stroke.Add(3);
            Stroke.Add(4);
            this.StrokeDotIndex.Add(Stroke);

            Stroke = new List<int>();
            Stroke.Add(1);
            Stroke.Add(5);
            Stroke.Add(6);
            this.StrokeDotIndex.Add(Stroke);

            Stroke = new List<int>();
            Stroke.Add(3);
            Stroke.Add(7);
            Stroke.Add(8);
            this.StrokeDotIndex.Add(Stroke);

            Stroke = new List<int>();
            Stroke.Add(9);
            Stroke.Add(10);
            Stroke.Add(11);
            Stroke.Add(12);
            Stroke.Add(13);
            this.StrokeDotIndex.Add(Stroke);
        }

        public override string getPath()
        {
            return "con_13.png";
        }
    }

    public class Hieong : CharacterBase
    {
        public Hieong()
        {
            this.DotList = new List<Point>();
            this.StrokeDotIndex = new List<List<int>>();
            this.CharacterName = "히흫";
            setDotList();
            setStrokeDotIndex();
        }
        public override void setDotList()
        {
            this.DotList.Add(new Point(350, 200));
            this.DotList.Add(new Point(450, 200));
            this.DotList.Add(new Point(550, 200));

            this.DotList.Add(new Point(200, 280));
            this.DotList.Add(new Point(325, 280));
            this.DotList.Add(new Point(450, 280));
            this.DotList.Add(new Point(575, 280));
            this.DotList.Add(new Point(700, 280));

            this.DotList.Add(new Point(450, 350));
            this.DotList.Add(new Point(400, 364));
            this.DotList.Add(new Point(364, 400));
            this.DotList.Add(new Point(350, 450));
            this.DotList.Add(new Point(364, 500));
            this.DotList.Add(new Point(400, 536));
            this.DotList.Add(new Point(450, 550));
            this.DotList.Add(new Point(500, 536));
            this.DotList.Add(new Point(536, 500));
            this.DotList.Add(new Point(550, 450));
            this.DotList.Add(new Point(536, 400));
            this.DotList.Add(new Point(500, 364));


        }
        public override void setStrokeDotIndex()
        {
            List<int> Stroke = new List<int>();

            Stroke.Add(0);
            Stroke.Add(1);
            Stroke.Add(2);
            this.StrokeDotIndex.Add(Stroke);

            Stroke = new List<int>();
            Stroke.Add(3);
            Stroke.Add(4);
            Stroke.Add(5);
            Stroke.Add(6);
            Stroke.Add(7);
            this.StrokeDotIndex.Add(Stroke);

            Stroke = new List<int>();
            Stroke.Add(8);
            Stroke.Add(9);
            Stroke.Add(10);
            Stroke.Add(11);
            Stroke.Add(12);
            Stroke.Add(13);
            Stroke.Add(14);
            Stroke.Add(15);
            Stroke.Add(16);
            Stroke.Add(17);
            Stroke.Add(18);
            Stroke.Add(19);
            Stroke.Add(8);
            this.StrokeDotIndex.Add(Stroke);
        }

        public override string getPath()
        {
            return "con_14.png";
        }
    }

    public class Ah : CharacterBase
    {
        public Ah()
        {
            this.DotList = new List<Point>();
            this.StrokeDotIndex = new List<List<int>>();
            this.CharacterName = "ㅏ(아)";
            setDotList();
            setStrokeDotIndex();
        }
        public override void setDotList()
        {
            this.DotList.Add(new Point(450, 130));
            this.DotList.Add(new Point(500, 200));
            this.DotList.Add(new Point(500, 300));
            this.DotList.Add(new Point(500, 400));
            this.DotList.Add(new Point(500, 500));
            this.DotList.Add(new Point(500, 600));

            this.DotList.Add(new Point(650, 300));
        }
        public override void setStrokeDotIndex()
        {
            List<int> Stroke = new List<int>();

            Stroke.Add(0);
            Stroke.Add(1);
            Stroke.Add(2);
            Stroke.Add(3);
            Stroke.Add(4);
            Stroke.Add(5);
            this.StrokeDotIndex.Add(Stroke);

            Stroke = new List<int>();
            Stroke.Add(2);
            Stroke.Add(6);
            this.StrokeDotIndex.Add(Stroke);
        }

        public override string getPath()
        {
            return "vow_1.png";
        }
    }

    public class Yah : CharacterBase
    {
        public Yah()
        {
            this.DotList = new List<Point>();
            this.StrokeDotIndex = new List<List<int>>();
            this.CharacterName = "ㅑ(야)";
            setDotList();
            setStrokeDotIndex();
        }
        public override void setDotList()
        {
            this.DotList.Add(new Point(450, 130));
            this.DotList.Add(new Point(500, 200));
            this.DotList.Add(new Point(500, 300));
            this.DotList.Add(new Point(500, 400));
            this.DotList.Add(new Point(500, 500));
            this.DotList.Add(new Point(500, 600));

            this.DotList.Add(new Point(650, 300));
            this.DotList.Add(new Point(650, 400));
        }
        public override void setStrokeDotIndex()
        {
            List<int> Stroke = new List<int>();

            Stroke.Add(0);
            Stroke.Add(1);
            Stroke.Add(2);
            Stroke.Add(3);
            Stroke.Add(4);
            Stroke.Add(5);
            this.StrokeDotIndex.Add(Stroke);

            Stroke = new List<int>();
            Stroke.Add(2);
            Stroke.Add(6);
            this.StrokeDotIndex.Add(Stroke);

            Stroke = new List<int>();
            Stroke.Add(3);
            Stroke.Add(7);
            this.StrokeDotIndex.Add(Stroke);
        }

        public override string getPath()
        {
            return "vow_2.png";
        }
    }

    public class Erh : CharacterBase
    {
        public Erh()
        {
            this.DotList = new List<Point>();
            this.StrokeDotIndex = new List<List<int>>();
            this.CharacterName = "ㅓ(어)";
            setDotList();
            setStrokeDotIndex();
        }
        public override void setDotList()
        {
            this.DotList.Add(new Point(450, 300));
            this.DotList.Add(new Point(600, 300));

            this.DotList.Add(new Point(550, 130));
            this.DotList.Add(new Point(600, 200));
            this.DotList.Add(new Point(600, 400));
            this.DotList.Add(new Point(600, 500));
            this.DotList.Add(new Point(600, 600));
        }
        public override void setStrokeDotIndex()
        {
            List<int> Stroke = new List<int>();

            Stroke.Add(0);
            Stroke.Add(1);
            this.StrokeDotIndex.Add(Stroke);

            Stroke = new List<int>();
            Stroke.Add(2);
            Stroke.Add(3);
            Stroke.Add(1);
            Stroke.Add(4);
            Stroke.Add(5);
            Stroke.Add(6);
            this.StrokeDotIndex.Add(Stroke);
        }

        public override string getPath()
        {
            return "vow_3.png";
        }
    }

    public class Yerh : CharacterBase
    {
        public Yerh()
        {
            this.DotList = new List<Point>();
            this.StrokeDotIndex = new List<List<int>>();
            this.CharacterName = "ㅕ(여)";
            setDotList();
            setStrokeDotIndex();
        }
        public override void setDotList()
        {
            this.DotList.Add(new Point(450, 300));
            this.DotList.Add(new Point(600, 300));

            this.DotList.Add(new Point(450, 400));
            this.DotList.Add(new Point(600, 400));

            this.DotList.Add(new Point(550, 130));
            this.DotList.Add(new Point(600, 200));
            this.DotList.Add(new Point(600, 500));
            this.DotList.Add(new Point(600, 600));
        }
        public override void setStrokeDotIndex()
        {
            List<int> Stroke = new List<int>();

            Stroke.Add(0);
            Stroke.Add(1);
            this.StrokeDotIndex.Add(Stroke);

            Stroke = new List<int>();
            Stroke.Add(2);
            Stroke.Add(3);
            this.StrokeDotIndex.Add(Stroke);

            Stroke = new List<int>();
            Stroke.Add(4);
            Stroke.Add(5);
            Stroke.Add(1);
            Stroke.Add(3);
            Stroke.Add(6);
            Stroke.Add(7);
            this.StrokeDotIndex.Add(Stroke);
        }

        public override string getPath()
        {
            return "vow_4.png";
        }
    }

    public class Oh : CharacterBase
    {
        public Oh()
        {
            this.DotList = new List<Point>();
            this.StrokeDotIndex = new List<List<int>>();
            this.CharacterName = "ㅗ(오)";
            setDotList();
            setStrokeDotIndex();
        }
        public override void setDotList()
        {
            this.DotList.Add(new Point(400, 265));
            this.DotList.Add(new Point(450, 300));
            this.DotList.Add(new Point(450, 380));
            this.DotList.Add(new Point(450, 460));

            this.DotList.Add(new Point(250, 460));
            this.DotList.Add(new Point(350, 460));
            this.DotList.Add(new Point(550, 460));
            this.DotList.Add(new Point(650, 460));
        }
        public override void setStrokeDotIndex()
        {
            List<int> Stroke = new List<int>();

            Stroke.Add(0);
            Stroke.Add(1);
            Stroke.Add(2);
            Stroke.Add(3);
            this.StrokeDotIndex.Add(Stroke);

            Stroke = new List<int>();

            Stroke.Add(4);
            Stroke.Add(5);
            Stroke.Add(3);
            Stroke.Add(6);
            Stroke.Add(7);
            this.StrokeDotIndex.Add(Stroke);
        }

        public override string getPath()
        {
            return "vow_5.png";
        }
    }

    public class Yoh : CharacterBase
    {
        public Yoh()
        {
            this.DotList = new List<Point>();
            this.StrokeDotIndex = new List<List<int>>();
            this.CharacterName = "ㅛ(요)";
            setDotList();
            setStrokeDotIndex();
        }
        public override void setDotList()
        {
            this.DotList.Add(new Point(350, 300));
            this.DotList.Add(new Point(350, 380));
            this.DotList.Add(new Point(350, 460));

            this.DotList.Add(new Point(550, 300));
            this.DotList.Add(new Point(550, 380));
            this.DotList.Add(new Point(550, 460));

            this.DotList.Add(new Point(250, 460));
            this.DotList.Add(new Point(450, 460));
            this.DotList.Add(new Point(650, 460));
        }
        public override void setStrokeDotIndex()
        {
            List<int> Stroke = new List<int>();

            Stroke.Add(0);
            Stroke.Add(1);
            Stroke.Add(2);
            this.StrokeDotIndex.Add(Stroke);

            Stroke = new List<int>();

            Stroke.Add(3);
            Stroke.Add(4);
            Stroke.Add(5);
            this.StrokeDotIndex.Add(Stroke);

            Stroke = new List<int>();
            Stroke.Add(6);
            Stroke.Add(2);
            Stroke.Add(7);
            Stroke.Add(5);
            Stroke.Add(8);
            this.StrokeDotIndex.Add(Stroke);
        }

        public override string getPath()
        {
            return "vow_6.png";
        }
    }

    public class Ooh : CharacterBase
    {
        public Ooh()
        {
            this.DotList = new List<Point>();
            this.StrokeDotIndex = new List<List<int>>();
            this.CharacterName = "ㅜ(우)";
            setDotList();
            setStrokeDotIndex();
        }
        public override void setDotList()
        {
            this.DotList.Add(new Point(250, 260));
            this.DotList.Add(new Point(350, 260));
            this.DotList.Add(new Point(450, 260));
            this.DotList.Add(new Point(550, 260));
            this.DotList.Add(new Point(650, 260));

            this.DotList.Add(new Point(450, 380));
            this.DotList.Add(new Point(450, 500));
        }
        public override void setStrokeDotIndex()
        {
            List<int> Stroke = new List<int>();

            Stroke.Add(0);
            Stroke.Add(1);
            Stroke.Add(2);
            Stroke.Add(3);
            Stroke.Add(4);
            this.StrokeDotIndex.Add(Stroke);

            Stroke = new List<int>();
            Stroke.Add(2);
            Stroke.Add(5);
            Stroke.Add(6);
            this.StrokeDotIndex.Add(Stroke);
        }

        public override string getPath()
        {
            return "vow_7.png";
        }
    }

    public class Yuh : CharacterBase
    {
        public Yuh()
        {
            this.DotList = new List<Point>();
            this.StrokeDotIndex = new List<List<int>>();
            this.CharacterName = "ㅠ(유)";
            setDotList();
            setStrokeDotIndex();
        }
        public override void setDotList()
        {
            this.DotList.Add(new Point(250, 260));
            this.DotList.Add(new Point(350, 260));
            this.DotList.Add(new Point(450, 260));
            this.DotList.Add(new Point(550, 260));
            this.DotList.Add(new Point(650, 260));

            this.DotList.Add(new Point(350, 380));
            this.DotList.Add(new Point(350, 500));

            this.DotList.Add(new Point(550, 380));
            this.DotList.Add(new Point(550, 500));
        }
        public override void setStrokeDotIndex()
        {
            List<int> Stroke = new List<int>();

            Stroke.Add(0);
            Stroke.Add(1);
            Stroke.Add(2);
            Stroke.Add(3);
            Stroke.Add(4);
            this.StrokeDotIndex.Add(Stroke);

            Stroke = new List<int>();
            Stroke.Add(1);
            Stroke.Add(5);
            Stroke.Add(6);
            this.StrokeDotIndex.Add(Stroke);

            Stroke = new List<int>();
            Stroke.Add(3);
            Stroke.Add(7);
            Stroke.Add(8);
            this.StrokeDotIndex.Add(Stroke);
        }

        public override string getPath()
        {
            return "vow_8.png";
        }
    }

    public class Uh : CharacterBase
    {
        public Uh()
        {
            this.DotList = new List<Point>();
            this.StrokeDotIndex = new List<List<int>>();
            this.CharacterName = "ㅡ(으)";
            setDotList();
            setStrokeDotIndex();
        }
        public override void setDotList()
        {
            this.DotList.Add(new Point(200, 350));
            this.DotList.Add(new Point(300, 350));
            this.DotList.Add(new Point(400, 350));
            this.DotList.Add(new Point(500, 350));
            this.DotList.Add(new Point(600, 350));
            this.DotList.Add(new Point(700, 350));
        }
        public override void setStrokeDotIndex()
        {
            List<int> Stroke = new List<int>();

            Stroke.Add(0);
            Stroke.Add(1);
            Stroke.Add(2);
            Stroke.Add(3);
            Stroke.Add(4);
            Stroke.Add(5);
            this.StrokeDotIndex.Add(Stroke);
        }

        public override string getPath()
        {
            return "vow_9.png";
        }
    }

    public class Eh : CharacterBase
    {
        public Eh()
        {
            this.DotList = new List<Point>();
            this.StrokeDotIndex = new List<List<int>>();
            this.CharacterName = "ㅣ(이)";
            setDotList();
            setStrokeDotIndex();
        }
        public override void setDotList()
        {
            this.DotList.Add(new Point(450, 150));
            this.DotList.Add(new Point(500, 200));
            this.DotList.Add(new Point(500, 300));
            this.DotList.Add(new Point(500, 400));
            this.DotList.Add(new Point(500, 500));
            this.DotList.Add(new Point(500, 600));
        }
        public override void setStrokeDotIndex()
        {
            List<int> Stroke = new List<int>();

            Stroke.Add(0);
            Stroke.Add(1);
            Stroke.Add(2);
            Stroke.Add(3);
            Stroke.Add(4);
            Stroke.Add(5);
            this.StrokeDotIndex.Add(Stroke);
        }

        public override string getPath()
        {
            return "vow_10.png";
        }
    }
}
