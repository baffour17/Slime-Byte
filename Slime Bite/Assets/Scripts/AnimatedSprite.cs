using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimatedSprite : MonoBehaviour
{
    // Start is called before the first frame update
    public SpriteRenderer spriteRenderer { get; private set;}

    public Sprite[] sprites;

    public float animationTime = 0.25f;
    public int animationFrame { get; private set; }
    public bool loop = true;

    private void Awake()
    {
        this.spriteRenderer = GetComponent<SpriteRenderer>();
    }


    private void Start()
    {
        InvokeRepeating(nameof(Advance), this.animationTime, this.animationTime);
    }

    private void Advance()
    {
        if (!this.spriteRenderer.enabled) {
            return;
        }

        this.animationFrame++;

        if (this.animationFrame >= this.sprites.Length && this.loop) {
            this.animationFrame = 0;
        }

        if (this.animationFrame >= 0 && this.animationFrame < this.sprites.Length){
            this.spriteRenderer.sprite = this.sprites[this.animationFrame];
        }
    }

    //way to restart animation
    public void Restart()
    {
        this.animationFrame =- 1;

        Advance();
    }
}