// (c) Copyright HutongGames, LLC 2010-2013. All rights reserved.


using System.Collections.Generic;
using HutongGames.PlayMaker;
using UnityEngine.UI;
using UnityEngine;

namespace Efs.PlayMaker.Actions
{

    [ActionCategory("UGUI")]
    [HutongGames.PlayMaker.Tooltip("Set an image's sprite.")]
    public class SetImageSprite : FsmStateAction
    {
        public FsmBool UseSelf;
        public Image Image;
        public FsmObject SpriteObject;
        public Sprite Sprite;


        public override void Reset()
        {
            Image = null;
            SpriteObject = null;
            Sprite = null;
            UseSelf = false;
        }
        
        public override void OnEnter()
        {
            var image = Image;
            if (UseSelf.Value)
                image = Owner.gameObject.GetComponent<Image>();

            var sprite = Sprite;
            if (SpriteObject.Value != null)
                sprite = SpriteObject.Value as Sprite;

            image.sprite = sprite;
            Finish();
        }
    }

    [ActionCategory("UGUI")]
    [HutongGames.PlayMaker.Tooltip("Get an image's sprite.")]
    public class GetImageSprite : FsmStateAction
    {
        public FsmBool UseSelf;
        public Image Image;

        [HutongGames.PlayMaker.ObjectType(typeof(Image))]
        public FsmObject Store;


        public override void Reset()
        {
            Store = null;
            UseSelf = false;
            Image = null;
        }

        public override void OnEnter()
        {
            var image = Image;
            if (UseSelf.Value)
                image = Owner.gameObject.GetComponent<Image>();

            Store.Value = image;
            Finish();
        }
    }

    [ActionCategory("GameObject")]
    [HutongGames.PlayMaker.Tooltip("Get a component's gameobject.")]
    public class GetGameObjectFromComponent : FsmStateAction
    {
        public FsmObject Component;
        public FsmGameObject Store;


        public override void Reset()
        {
            Component = null;
            Store = null;
        }

        public override void OnEnter()
        {
            var comp = (MonoBehaviour)Component.Value;
            Store.Value = comp.gameObject;
            Finish();
        }
    }
}