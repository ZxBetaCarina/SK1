using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(SpriteRenderer))]
public class Icons : MonoBehaviour, IEqualityComparer<Icons>, IEquatable<Icons>
{
    public string Name;
    public Sprite IconSprite { get { return GetComponent<SpriteRenderer>().sprite; } }

    public Sprite GetSprite()
    {
        return IconSprite;
    }

    public bool Equals(Icons x, Icons y)
    {
        return x.IconSprite == y.IconSprite || x.Name == y.Name;
    }

    public int GetHashCode(Icons obj)
    {
        return obj.GetHashCode();
    }

    public bool Equals(Icons other)
    {
        return IconSprite == other.IconSprite || Name == other.Name;
    }
}
