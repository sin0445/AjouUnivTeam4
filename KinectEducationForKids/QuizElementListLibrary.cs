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

        public static QuizElements getQuizElements(QUIZTYPE type)
        {
            List<string> QuizElementList = new List<string>();
            
            QuizElements elements;

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

                    elements = new QuizElements(QuizElementList);
                    break;
                case QUIZTYPE.FRUIT:
                    elements = null;
                    break;
                default:
                    elements = null;
                    
                    break;
            }
            return elements;
        }
    }

    public class QuizElements
    {
        public List<string> QuizElementList;
        public List<string> QuizButtonPathList;
        public List<string> QuizImagePathList;

        // 문제 중복 방지
        private List<bool> QuizDuplicateList;

        // 실제 문제 elements path
        public List<string> QuizList;

        // element 모두 한 문제씩 출제되는지 확인하기 위함
        public int QuizCount;

        // 정답 element
        private int AnswerNum;

        // 정답 element 보기 위치
        private int LocateAnswer;

        // 오답 element
        private int[] WrongNum;

        public QuizElements(List<string> elementList)
        {
            this.QuizCount = 0;
            this.WrongNum = new int[3];
            this.QuizElementList = elementList;
            this.QuizButtonPathList = addButtonsPath(elementList);
            this.QuizImagePathList = addImagesPath(elementList);
            this.QuizDuplicateList = new List<bool>();
            for (int i = 0; i < this.QuizElementList.Count; i++)
            {
                this.QuizDuplicateList.Add(false);
            }
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

        public void SetQuiz()
        {
            Generating();

            QuizCount++;

            QuizList = new List<string>();

            // 문제 Element 경로 저장
            QuizList.Add(QuizImagePathList[AnswerNum]);

            switch (LocateAnswer)
            {
                case 0:
                    {
                        // 정답 element 보기 위치가 1번인 경우
                        QuizList.Add(QuizButtonPathList[AnswerNum]);
                        QuizList.Add(QuizButtonPathList[WrongNum[0]]);
                        QuizList.Add(QuizButtonPathList[WrongNum[1]]);
                        QuizList.Add(QuizButtonPathList[WrongNum[2]]);
                    }
                    break;
                case 1:
                    {
                        // 정답 element 보기 위치가 2번인 경우
                        QuizList.Add(QuizButtonPathList[WrongNum[0]]);
                        QuizList.Add(QuizButtonPathList[AnswerNum]);
                        QuizList.Add(QuizButtonPathList[WrongNum[1]]);
                        QuizList.Add(QuizButtonPathList[WrongNum[2]]);
                    }
                    break;
                case 2:
                    {
                        // 정답 element 보기 위치가 3번인 경우
                        QuizList.Add(QuizButtonPathList[WrongNum[0]]);
                        QuizList.Add(QuizButtonPathList[WrongNum[1]]);
                        QuizList.Add(QuizButtonPathList[AnswerNum]);
                        QuizList.Add(QuizButtonPathList[WrongNum[2]]);
                    }
                    break;
                case 3:
                    {
                        // 정답 element 보기 위치가 4번인 경우
                        QuizList.Add(QuizButtonPathList[WrongNum[0]]);
                        QuizList.Add(QuizButtonPathList[WrongNum[1]]);
                        QuizList.Add(QuizButtonPathList[WrongNum[2]]);
                        QuizList.Add(QuizButtonPathList[AnswerNum]);
                    }
                    break;
            }
            // 정답 element 보기 위치 저장
            QuizList.Add(LocateAnswer.ToString());
        }

        // 정답 element 생성 및 오답 element 생성
        public void Generating()
        {
            int max = this.QuizElementList.Count;

            System.Random random = new System.Random();

            bool flag = true;

            do
            {
                // 정답 element 생성
                AnswerNum = random.Next(0, max);

                // 기존 문제 중복 확인
                if (this.QuizDuplicateList[AnswerNum] == false)
                {
                    this.QuizDuplicateList[AnswerNum] = true;

                    flag = false;
                }
            }
            while (flag);

            // 정답 element 보기 위치 생성
            LocateAnswer = random.Next(0, 4);

            // 오답 element 값 초기화
            for (int i = 0; i < WrongNum.Length; i++)
            {
                WrongNum[i] = -1;
            }

            flag = true;
            do
            {
                // 오답1 element 생성
                WrongNum[0] = random.Next(0, max);

                // 오답1 element 중복 확인
                if ((WrongNum[0] != AnswerNum) && (WrongNum[0] != WrongNum[1]) && (WrongNum[0] != WrongNum[2]))
                {
                    flag = false;
                }
            }
            while (flag);

            flag = true;
            do
            {
                // 오답2 element 생성
                WrongNum[1] = random.Next(0, max);

                // 오답2 element 중복 확인
                if ((WrongNum[1] != AnswerNum) && (WrongNum[1] != WrongNum[0]) && (WrongNum[1] != WrongNum[2]))
                {
                    flag = false;
                }
            }
            while (flag);

            flag = true;
            do
            {
                // 오답3 element 생성
                WrongNum[2] = random.Next(0, max);

                // 오답3 element 중복 확인
                if ((WrongNum[2] != AnswerNum) && (WrongNum[2] != WrongNum[0]) && (WrongNum[2] != WrongNum[1]))
                {
                    flag = false;
                }
            }
            while (flag);
        }

        // 모든 문제 출제 됐는지 확인 하는 함수
        public bool isGameEnd()
        {
            if (this.QuizCount >= this.QuizElementList.Count)
            {
                return true;
            }
            return false;
        }
    }
}
