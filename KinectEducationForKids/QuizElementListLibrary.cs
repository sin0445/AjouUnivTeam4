using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Random;

namespace KinectEducationForKids
{
    public static class QuizElementListLibrary
    {
        public enum QUIZTYPE
        {
            ANIMAL = 1,
            FRUIT = 2
        };

        public static List<string> getQuizElements(QUIZTYPE type)
        {
            List<string> QuizElementList = new List<string>();

            switch(type)
            {
                case QUIZTYPE.ANIMAL:
                    QuizElementList.Add("elephant");
                    QuizElementList.Add("giraffe");
                    QuizElementList.Add("hippo");
                    QuizElementList.Add("lion");
                    QuizElementList.Add("monkey");
                    QuizElementList.Add("penguin");
                    QuizElementList.Add("pig");
                    QuizElementList.Add("rabbit");
                    QuizElementList.Add("racoon");
                    break;
                case QUIZTYPE.FRUIT:
                    break;
                default:
                    QuizElementList = null;
                    break;
            }
            return QuizElementList;
        }
    }

    public class QuizElements
    {
        public List<string> QuizElementList;
        public List<string> QuizButtonPathList;
        public List<string> QuizImagePathList;

        public int QuizCount;
        public int AnswerNum;
        public int[] WrongNum;

        public QuizElements(List<string> elementList)
        {
            this.QuizCount = 0;
            this.WrongNum = new int[3];
            this.QuizElementList = elementList;
            this.QuizButtonPathList =addButtonsPath(elementList);
            this.QuizImagePathList = addImagesPath(elementList);
        }
        private List<string> addButtonsPath(List<string> elementList)
        {
            List<string> btnPathList = new List<string>();

            for (int i = 0; i < elementList.Count; i++)
            {
                btnPathList.Add("btn_" + elementList[i] + ".png");
            }
            return btnPathList;
        }
        private List<string> addImagesPath(List<string> elementList)
        {
            List<string> imgPathList = new List<string>();
            for (int i = 0; i < elementList.Count; i++)
            {
                imgPathList.Add("img_" + elementList[i] + ".png");
            }
            return imgPathList;
        }

        public void MakeQuiz()
        {

        }


    }
}
