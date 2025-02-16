using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TutorialActions : TextFieldController
{
    [SerializeField]
    GameManager gameManager;
    
    [SerializeField]
    StoneManager stoneManager;

    [SerializeField]
    TutorialPlayerMe tutorialPlayer;

    [SerializeField]
    TutorialPlayerEnemy tutorialEnemy;

    [HideInInspector]
    public int currentProcess = 0;

    [SerializeField]
    AudioSource audioSource;

    AudioClip nextTextSound;

    private void Awake()
    {
        Data.Instance.isTutorial = true;
    }

    override protected void InitializeProcess()
    {
        //0 �Q�[���J�n
        fWaits.Add(() => { gameManager.GameStart(); return true; });

        //1 �t���O�ݒ�1
        fWaits.Add(() => { currentProcess = 1; return true; });

        //2 �ېΑI��
        fWaits.Add(() => { return currentProcess == 2; });

        //3 �t���O�ݒ�2
        fWaits.Add(() => { currentProcess = 3; return true; });

        //4 �ېΔz�u
        fWaits.Add(() => { return currentProcess == 4; });

        //5 �t���O�ݒ�3
        fWaits.Add(() => { currentProcess = 5; return true; });

        //6 �G �X�L�������p �ʏ�� �z�u
        fWaits.Add(() => { return currentProcess == 6; });

        //7 �t���O�ݒ�4
        fWaits.Add(() => { stoneManager.SetStone(EStone.DEFAULT, 4, 2, ETeam.WHITE); stoneManager.Stones[1, 2].SetTeam(ETeam.WHITE); currentProcess = 7; return true; });

        //8 �Ζ߂�
        fWaits.Add(() => { return currentProcess == 8; });

        //9 �t���O�ݒ�5
        fWaits.Add(() => { currentProcess = 9; stoneManager.Stones[4, 3].SetTeam(ETeam.BLACK); return true; });

        //10 �R���{�p �G �ې� �z�u
        fWaits.Add(() => { return currentProcess == 10; });

        //11 �t���O�ݒ�6
        fWaits.Add(() => { currentProcess = 11; return true; });

        //12 �R���{���� �R���{�{�[�i�X��O
        fWaits.Add(() => { return currentProcess == 12; });

        //13 �t���O�ݒ�7
        fWaits.Add(() => { currentProcess = 13; return true; });

        //14 �R���{�I��
        fWaits.Add(() => { return currentProcess == 14; });

        //15 �^�C�g����
        fWaits.Add(() => { SceneManager.LoadScene(Data.Instance.TITLE_SCENE_NAME); Data.Instance.isTutorial = false; return true; });

        nextTextSound = Resources.Load<AudioClip>("Sound/Text/TextNext");

        //var s = Path.Combine(Application.streamingAssetsPath, "Text\\Tutorial\\Text.txt");

        //encoding = System.Text.Encoding.GetEncoding("shift_jis");

        stoneManager.SetStone(EStone.DEFAULT, 3, 2, ETeam.WHITE);
        stoneManager.Stones[3, 3].SetTeam(ETeam.WHITE);
        stoneManager.SetStone(EStone.CIRCLE, 5, 4, ETeam.WHITE, true);

        Init("");
    }

    protected override bool PressAnyKey()
    {
        if ((Input.touches.Length > 0) || Input.GetMouseButtonDown(0))
        {
            audioSource.PlayOneShot(nextTextSound);
            return true;
        }
        return false;
    }

    public bool OnSelectStone(EStone stone)
    {
        if(currentProcess==1 && stone == EStone.CIRCLE)
        {
            currentProcess = 2;
            return true;
        }

        return false;
    }

    public bool OnSelectCell(int x, int y)
    {
        if(currentProcess==3 && x==2&&y==2)
        {
            currentProcess = 4;
            tutorialPlayer.highlightList = new List<Vector2> { new Vector2(0, 2) };
            return true;
        }

        if (currentProcess == 7 && x == 0 && y == 2)
        {
            currentProcess = 8;
            tutorialPlayer.highlightList = new List<Vector2> { new Vector2(2, 4) };
            return true;
        }

        if (currentProcess == 11 && x == 2 && y == 4)
        {
            tutorialPlayer.highlightList = new List<Vector2> { new Vector2(5, 4) };
            return true;
        }


        return false;
    }

    public bool EnemyOnSelectCell(out int x, out int y, out EStone putStone)
    {
        if(currentProcess==5)
        {
            x = 1;
            y = 2;
            putStone = EStone.DEFAULT;
            return true;
            
        }

        if (currentProcess == 9)
        {
            x = 2;
            y = 3;
            putStone = EStone.CIRCLE;
            return true;

        }

        x = 0;
        y = 0;
        putStone = EStone.NONE;
        return false;
    }

    public void OnPutStoneEnd()
    {
        if (currentProcess == 5) 
        {
            currentProcess = 6;
        }

        if (currentProcess == 9)
        {
            currentProcess = 10;
        }

        if(currentProcess == 13)
        {
            currentProcess = 14;
        }
    }

    public bool OnComboBonus(int x, int y)
    {
        if(currentProcess == 11)
        {
            currentProcess = 12;
        }

        if (currentProcess == 13 && x == 5 && y == 4) 
        {
            return true;
        }

        return false;
    }
}
