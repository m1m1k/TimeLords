using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

/// <summary>
/// NOTE: Unity "tags" should be equal to this Enum CreatureVerb
/// </summary>
public enum CreatureVerb
{
    Move,
    Attack,
    Eat,
    Exit,
    Drink,
    Death
}

public class Verb
{
    CreatureVerb EnumVerb;
    public List<AudioClip> SoundVariants;
    public String Description;
    public Verb(CreatureVerb verb, AudioClip clip1, AudioClip clip2) : this(verb, new List<AudioClip> { clip1, clip2 })
    {    }
    public Verb(CreatureVerb verb, List<AudioClip> clips)
    {
        EnumVerb = verb;
        SoundVariants = clips;
    }
}

public class Player : MovingObject {

    public int weaponDamage = 1; //todo don't hardcode
    public int pointsPerFood = 10;
    public int pointsPerSoda = 20;
    public float restartLevelDelay = 1f;
    public Text foodText;

    public static AudioClip moveSound1;
    public static AudioClip moveSound2;
    public AudioClip chopSound1;
    public AudioClip chopSound2;
    public AudioClip eatSound1;
    public AudioClip eatSound2;
    public AudioClip drinkSound1;
    public AudioClip drinkSound2;
    public AudioClip gameOverSound1;
    private Dictionary<CreatureVerb, Verb> Sounds = new Dictionary<CreatureVerb, Verb>
    {
        { CreatureVerb.Move, new Verb(CreatureVerb.Move, moveSound1, moveSound2) }
    };


    private Animator animator;
    private int foodPointsRemaining;
    


    // Use this for initialization
    protected override void Start() {
        animator = GetComponent<Animator>();
        foodPointsRemaining = GameManager.Instance.playerFoodPoints;
        UpdateFoodText();

        base.Start();
    }

    private void UpdateFoodText(params string[] args)
    {
        foodText.text = String.Format("Food: {0} {1}", foodPointsRemaining, String.Join(" ", args));
    }

    private void OnDisable()
    {
        GameManager.Instance.playerFoodPoints = foodPointsRemaining;
    }

    private void CheckIfGameOver()
    {
        if (foodPointsRemaining <= 0)
        {
            SoundManager.instance.PlaySingle(gameOverSound1);
            SoundManager.instance.musicSource.Stop();
            GameManager.Instance.GameOver();
        }
    }

    // Update is called once per frame
    void Update() {
        if (!GameManager.Instance.playersTurn)
        {
            return;
        }
        int horizontal = (int)Input.GetAxisRaw("Horizontal");
        int vertical = (int)Input.GetAxisRaw("Vertical");
        if (horizontal != 0)
        {
            vertical = 0; // no diagonals
        }
        if (horizontal != 0 || vertical != 0)
        {
            AttemptMove<ModifyTerrain>(horizontal, vertical);
        }
    }

    protected override void AttemptMove<T>(int xDir, int yDir)
    {
        foodPointsRemaining -= 1;
        UpdateFoodText();
        base.AttemptMove<T>(xDir, yDir);
        RaycastHit2D hit;
        if(Move(xDir, yDir, out hit))
        {
            SoundManager.instance.RandomizeSfx(moveSound1, moveSound2);
        }

        CheckIfGameOver();
        GameManager.Instance.playersTurn = false; //turn has ended.
    }

    //OnCantMove overrides the abstract function OnCantMove in MovingObject.
    //It takes a generic parameter T which in the case of Player is a Wall which the player can attack and destroy.
    protected override void OnCantMove<T>(T component)
    {
        //Set hitWall to equal the component passed in as a parameter.
        ModifyTerrain hitWall = component as ModifyTerrain;

        //Call the DamageWall function of the Wall we are hitting.
        hitWall.DamageWall(weaponDamage);

        //Set the attack trigger of the player's animation controller in order to play the player's attack animation.
        animator.SetTrigger("playerChop");
    }

    /// <summary>
    /// On Exit, restart the level
    /// </summary>
    private void Restart()
    {
        //Load the last scene loaded, in this case Main, the only scene in the game.
        SceneManager.LoadScene(0);
    }

    //OnTriggerEnter2D is sent when another object enters a trigger collider attached to this object (2D physics only).
    private void OnTriggerEnter2D(Collider2D other)
    {
        // NOTE: tags should be equal to Enum CreatureVerb
        var verb = (CreatureVerb)Enum.Parse(typeof(CreatureVerb), other.tag);
        switch(verb)
        {
            case CreatureVerb.Exit:
                //Invoke the Restart function to start the next level with a delay of restartLevelDelay (default 1 second).
                Invoke("Restart", restartLevelDelay);
                //Disable the player object since level is over.
                enabled = false;
                break;
            case CreatureVerb.Eat:
            case CreatureVerb.Drink:
                SoundManager.instance.RandomizeSfx(Sounds[verb].SoundVariants[0], Sounds[verb].SoundVariants[1]);
                UpdateFoodText("+ " + pointsPerFood);
                foodPointsRemaining += pointsPerFood;
                other.gameObject.SetActive(false);
                break;
        }
        
    }

    public void LoseFood(int loss)
    {
        animator.SetTrigger("playerHit");
        foodPointsRemaining -= loss;
        UpdateFoodText("- " + loss);
        CheckIfGameOver();
    }
}