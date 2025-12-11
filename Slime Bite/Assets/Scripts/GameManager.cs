using System.Collections;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public Antislime[] antislimes;
    public Greenslime[] greenslimes;
    public Transform pellets;

    public int antislimeMultiplier { get; private set; } = 1;
    public int score { get; private set; }
    public int lives { get; private set; }

    private void Start()
    {
        NewGame();
    }

    private void Update()
    {
        if (this.lives <= 0 && Input.anyKeyDown)
        {
            NewGame();
        }
    }

    private void NewGame()
    {
        SetScore(0);
        SetLives(3);
        NewRound();
    }

    private void NewRound()
    {
        // Reactivate all pellets
        if (this.pellets != null)
        {
            foreach (Transform pellet in this.pellets)
            {
                if (pellet != null)
                    pellet.gameObject.SetActive(true);
            }
        }

        // Reactivate all green slimes
        if (this.greenslimes != null)
        {
            for (int i = 0; i < this.greenslimes.Length; i++)
            {
                if (this.greenslimes[i] != null)
                    this.greenslimes[i].gameObject.SetActive(true);
            }
        }

        // Reactivate all antislimes
        if (this.antislimes != null)
        {
            for (int i = 0; i < this.antislimes.Length; i++)
            {
                if (this.antislimes[i] != null)
                    this.antislimes[i].gameObject.SetActive(true);
            }
        }
    }

    // Reset antislime multiplier and reactivate enemies (but keep pellets as is)
    private void ResetState()
    {
        ResetantislimeMultiplier();

        if (this.antislimes != null)
        {
            for (int i = 0; i < this.antislimes.Length; i++)
            {
                if (this.antislimes[i] != null)
                    this.antislimes[i].gameObject.SetActive(true);
            }
        }

        if (this.greenslimes != null)
        {
            for (int i = 0; i < this.greenslimes.Length; i++)
            {
                if (this.greenslimes[i] != null)
                    this.greenslimes[i].gameObject.SetActive(true);
            }
        }
    }

    // If no lives left, game over
    private void GameOver()
    {
        if (this.antislimes != null)
        {
            for (int i = 0; i < this.antislimes.Length; i++)
            {
                if (this.antislimes[i] != null)
                    this.antislimes[i].gameObject.SetActive(false);
            }
        }

        if (this.greenslimes != null)
        {
            for (int i = 0; i < this.greenslimes.Length; i++)
            {
                if (this.greenslimes[i] != null)
                    this.greenslimes[i].gameObject.SetActive(false);
            }
        }
    }

    private void SetScore(int score)
    {
        this.score = score;
        // TODO: update UI
    }

    private void SetLives(int lives)
    {
        this.lives = lives;
        // TODO: update UI
    }

    public void AntiSlimeEaten(Antislime antislime)
    {
        if (antislime == null) return;

        int points = antislime.points * this.antislimeMultiplier;
        SetScore(this.score + points);

        // Increase multiplier for next antislime eaten
        this.antislimeMultiplier *= 2;
    }

    // If a green slime is eaten by the antislime
    public void GreenslimeEaten(Greenslime eaten)
    {
        if (eaten != null)
        {
            // call the greenslime's own OnEaten helper (if you created one)
            eaten.OnEaten();

            // start a respawn coroutine on the GameManager that uses the Greenslime's respawn coroutine
            StartCoroutine(RespawnGreenslime(eaten, 3f));
        }
        else
        {
            // fallback: disable all greenslimes
            if (this.greenslimes != null)
            {
                for (int i = 0; i < this.greenslimes.Length; i++)
                {
                    if (this.greenslimes[i] != null)
                        this.greenslimes[i].gameObject.SetActive(false);
                }
            }
        }

        if (this.lives > 0)
        {
            // schedule ResetState for the level after a short delay
            Invoke(nameof(ResetState), 3.0f);
        }
        else
        {
            GameOver();
        }
    }

    private IEnumerator RespawnGreenslime(Greenslime g, float delay)
    {
        yield return new WaitForSeconds(delay);

        // If Greenslime implements DoRespawn coroutine, use it.
        // Otherwise call its ResetState() directly.
        if (g != null)
        {
            // If Greenslime has a public IEnumerator DoRespawn(), start it on the Greenslime.
            var method = g.GetType().GetMethod("DoRespawn");
            if (method != null)
            {
                // start the Greenslime's coroutine method
                g.StartCoroutine((System.Collections.IEnumerator)method.Invoke(g, null));
            }
            else
            {
                g.ResetState();
            }
        }
    }

    // Called by pellets (when player collects a pellet)
    public void PelletEaten(Pellet pellet)
    {
        if (pellet == null) return;

        pellet.gameObject.SetActive(false);
        SetScore(this.score + pellet.points);

        if (!HasRemainingPellets())
        {
            // disable all greenslimes and start a new round after a short delay
            if (this.greenslimes != null)
            {
                for (int i = 0; i < this.greenslimes.Length; i++)
                {
                    if (this.greenslimes[i] != null)
                        this.greenslimes[i].gameObject.SetActive(false);
                }
            }

            Invoke(nameof(NewRound), 3.0f);
        }
    }

    public void PowerPelletEaten(PowerPellet pellet)
    {
        if (pellet == null) return;

        // Example: when a power pellet is eaten, make antislimes vulnerable (implementation TBD)
        // Reset multiplier after pellet.duration seconds (cancel previous ones)
        CancelInvoke(nameof(ResetantislimeMultiplier));
        Invoke(nameof(ResetantislimeMultiplier), pellet.duration);

        // Also count the pellet as eaten (points + deactivate)
        pellet.gameObject.SetActive(false);
        SetScore(this.score + pellet.points);
    }

    private bool HasRemainingPellets()
    {
        if (this.pellets == null) return false;

        foreach (Transform pellet in this.pellets)
        {
            if (pellet != null && pellet.gameObject.activeSelf)
            {
                return true;
            }
        }

        return false;
    }

    private void ResetantislimeMultiplier()
    {
        this.antislimeMultiplier = 1;
    }
}
