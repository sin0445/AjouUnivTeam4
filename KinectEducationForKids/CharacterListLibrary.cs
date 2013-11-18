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
            consList.Add(new Nieun());
            consList.Add(new Digeuk());
            return consList;
        }
        private static List<CharacterBase> getVowels()
        {
            List<CharacterBase> vowList = new List<CharacterBase>();
            //여기다 ㅏ 부터 ㅣ 까지 클래스를 정의한 후 위의 디귿과 같은 방식으로 추가시키면 된다.
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
        Kiyeok()
        {
            this.DotList = new List<Point>();
            this.StrokeDotIndex = new List<List<int>>();

            setDotList();
            setStrokeDotIndex();
        }
        public override void setDotList()
        {
        }
        public override void setStrokeDotIndex()
        {
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
            this.DotList.Add(new Point(100, 320));
            this.DotList.Add(new Point(100, 390));
            this.DotList.Add(new Point(100, 460));
            this.DotList.Add(new Point(250, 460));
            this.DotList.Add(new Point(400, 460));
            this.DotList.Add(new Point(550, 460));
            this.DotList.Add(new Point(700, 460));
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
            this.DotList.Add(new Point(100, 320));
            this.DotList.Add(new Point(100, 390));
            this.DotList.Add(new Point(100, 460));
            this.DotList.Add(new Point(250, 460));
            this.DotList.Add(new Point(400, 460));
            this.DotList.Add(new Point(550, 460));
            this.DotList.Add(new Point(700, 460));
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
}
