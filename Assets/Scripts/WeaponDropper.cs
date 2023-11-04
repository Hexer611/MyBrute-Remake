using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponDropper : MonoBehaviour
{
    [SerializeField] GameObject dropAnimation;
    [SerializeField] SpriteRenderer selfSpriteRenderer;
    [SerializeField] Rigidbody2D selfRigidBody;
    // Start is called before the first frame update
    public void Initialize(SpriteRenderer targetSpriteRenderer, Vector2 direction)
    {
        selfSpriteRenderer.sprite = targetSpriteRenderer.sprite;
        transform.position = targetSpriteRenderer.transform.position;
        transform.localScale = targetSpriteRenderer.transform.lossyScale;
        selfRigidBody.AddForce(direction * 10, ForceMode2D.Impulse);
        dropAnimation.transform.parent = null;
        dropAnimation.transform.localScale = Vector3.one;

        Destroy(gameObject, 2f);
        Destroy(dropAnimation, 2f);
    }

    // Update is called once per frame
    void Update()
    {
        selfSpriteRenderer.color -= new Color(0, 0, 0, Time.deltaTime);
    }
}
