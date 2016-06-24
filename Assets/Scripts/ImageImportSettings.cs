using UnityEngine;
using System.Collections;

public class ImageImportSettings : MonoBehaviour
{
    public static ImageImportSettings instance;
    public SpriteRenderer sprite;
    private StickyScript sticky;
    public SpriteButton leftRotateButton;
    public SpriteButton rightRotateButton;
    public SpriteButton okayButton;

    void Awake()
    {
        instance = this;
        gameObject.SetActive(false);
    }

    void Update()
    {
        if (rightRotateButton.pressed)
        {
            sticky.uneditedTex = StickyScript.rotateImage(sticky.uneditedTex, false);
            sticky.photoRotateCount--;
            sprite.transform.eulerAngles += Vector3.forward * 90f;
        }
        if (leftRotateButton.pressed)
        {
            sticky.uneditedTex = StickyScript.rotateImage(sticky.uneditedTex, true);
            sticky.photoRotateCount++;
            sprite.transform.eulerAngles -= Vector3.forward * 90f;
        }
        if (okayButton.pressed)
        {
            sticky.editTexForShape();
            gameObject.SetActive(false);
        }
    }

    public void setActive(StickyScript ss)
    {
        sprite.transform.eulerAngles = Vector3.zero;
        sticky = ss;
        //sticky.uneditedTex = StickyScript.rotateImage(sticky.uneditedTex, true);
        putSprite();
        gameObject.SetActive(true);
    }

    private void putSprite()
    {
        Rect rect = new Rect(0, 0, sticky.uneditedTex.width, sticky.uneditedTex.height);
        Vector2 vec2 = new Vector2(0.5f, 0.5f);
        sprite.sprite = Sprite.Create(sticky.uneditedTex, rect, vec2);
        sprite.enabled = true;
    }
}
