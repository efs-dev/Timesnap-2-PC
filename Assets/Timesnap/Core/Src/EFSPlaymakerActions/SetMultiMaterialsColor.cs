// (c) Copyright HutongGames, LLC 2010-2013. All rights reserved.

using UnityEngine;
using System.Collections.Generic;
using HutongGames.PlayMaker;
using HutongGames.PlayMaker.Actions;

namespace Efs.PlayMaker.Actions
{
	[ActionCategory(ActionCategory.Material)]
	[HutongGames.PlayMaker.Tooltip("Sets a named color value in a game object's material.")]
	public class SetMultiMaterialsColor : ComponentAction<Renderer>
	{
        [HutongGames.PlayMaker.Tooltip("The GameObjects that the material is applied to.")]
        [CheckForComponent(typeof(Renderer))]
        public FsmGameObject[] gameObjects;

		[HutongGames.PlayMaker.Tooltip("GameObjects can have multiple materials. Specify an index to target a specific material.")]
		public FsmInt materialIndex;
        
		[UIHint(UIHint.NamedColor)]
		[HutongGames.PlayMaker.Tooltip("A named color parameter in the shader.")]
		public FsmString namedColor;
		
		[RequiredField]
		[HutongGames.PlayMaker.Tooltip("Set the parameter value.")]
		public FsmColor color;

		[HutongGames.PlayMaker.Tooltip("Repeat every frame. Useful if the value is animated.")]
		public bool everyFrame;

		public override void Reset()
		{
            gameObjects = new FsmGameObject[3];
			materialIndex = 0;
			namedColor = "_Color";
			color = Color.black;
			everyFrame = false;
		}

		public override void OnEnter()
		{
			DoSetMaterialColor();
			
			if (!everyFrame)
				Finish();
		}
		
		public override void OnUpdate()
		{
			DoSetMaterialColor();
		}

		void DoSetMaterialColor()
		{
			if (color.IsNone)
			{
				return;
			}

			var colorName = namedColor.Value;
			if (colorName == "") colorName = "_Color";

            for (var i = 0; i < gameObjects.Length; i++)
            {
                var gameObject = gameObjects[i];
                if (gameObject == null)
                    continue;
                var renderer = gameObject.Value.GetComponent<Renderer>();

                if (renderer.materials.Length == 1)
                {
                    renderer.material.SetColor(colorName, color.Value);
                }
                else if (renderer.materials.Length > materialIndex.Value)
                {
                    var materials = renderer.materials;
                    materials[materialIndex.Value].SetColor(colorName, color.Value);
                    renderer.materials = materials;
                }
            }
		}
	}
}