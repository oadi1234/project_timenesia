using System.Collections.Generic;
using UnityEngine;

namespace _2_Scripts.Player.Animation.model
{
    //class for storing and getting location information on where in a given frame of animation cloak or hair should attach.
    // this one definitely needs to be changed to a json file
    [CreateAssetMenu(fileName = "pivotPerFrame", menuName = "ScriptableObjects/PivotConstants", order = 1)]
    public class PivotPerFrame : ScriptableObject
    {
        static PivotPerFrame _instance;

        public static PivotPerFrame Instance
        {
            get
            {
                if (!_instance)
                {
                    _instance = Resources.Load<PivotPerFrame>("pivotPerFrame");
                }

                return _instance;
            }
        }

        private readonly Dictionary<int, Dictionary<int, Vector2>> cloakPivotFramesPerState;
        private readonly Dictionary<int, Dictionary<int, Vector2>> hairPivotFramesPerState;
        private static HashSet<int> _incorrectStateList;

        public Vector2 GetCloakPivotLocation(int animationState, int animationFrame)
        {
            return GetPivotLocation(animationState, animationFrame, cloakPivotFramesPerState);
        }
        
        public Vector2 GetHairPivotLocation(int animationState, int animationFrame)
        {
            return GetPivotLocation(animationState, animationFrame, hairPivotFramesPerState);
        }

        public Vector2 GetPivotLocation(int animationState, int animationFrame,
            Dictionary<int, Dictionary<int, Vector2>> dict)
        {
            if (dict.ContainsKey(animationState))
            {
                if (!dict[animationState].ContainsKey(animationFrame))
                {
                    return dict[animationState][0];
                }
                return dict[animationState][animationFrame];
            }
            return Vector2.zero;
        }

        public bool IsIncorrectState(int state)
        {
            return _incorrectStateList.Contains(state);
        }

        public PivotPerFrame()
        {
            cloakPivotFramesPerState = new Dictionary<int, Dictionary<int, Vector2>>();
            hairPivotFramesPerState = new Dictionary<int, Dictionary<int, Vector2>>();
            PopulateCloakPivots();
            PopulateHairPivots();

            SetIncorrectHingeStateList();
        }

        private void PopulateHairPivots()
        {
            //Idle state
            Dictionary<int, Vector2> temp = new Dictionary<int, Vector2>();
            temp.Add(0, new Vector2(-0.039f, 1.37f));
            hairPivotFramesPerState.Add(AC.Idle, temp);

            //ascend
            temp = new Dictionary<int, Vector2>();
            temp.Add(0, new Vector2(-0.573f, 1.3f));
            hairPivotFramesPerState.Add(AC.Ascend, temp);

            //descent
            temp = new Dictionary<int, Vector2>();
            temp.Add(0, new Vector2(0.054f, 1.447f));
            hairPivotFramesPerState.Add(AC.Descent, temp);

            // hurt_heavy
            temp = new Dictionary<int, Vector2>();
            temp.Add(0, new Vector2(-0.96f, 0.665f));
            temp.Add(1, new Vector2(-1.045f, 0.835f));
            temp.Add(2, new Vector2(-0.96f, 0.928f));
            temp.Add(3, new Vector2(-0.929f, 0.943f));
            temp.Add(4, new Vector2(-0.72f, 1.082f));
            temp.Add(5, new Vector2(-0.588f, 1.252f));
            temp.Add(6, new Vector2(-0.363f, 1.376f));
            temp.Add(7, new Vector2(-0.309f, 1.384f));
            temp.Add(8, new Vector2(-0.309f, 1.399f));
            temp.Add(9, new Vector2(-0.34f, 1.407f));
            temp.Add(10, new Vector2(-0.34f, 1.407f));
            hairPivotFramesPerState.Add(AC.HurtHeavy, temp);

            // hurt_light
            temp = new Dictionary<int, Vector2>();
            temp.Add(0, new Vector2(-0.13f, 1.19f));
            temp.Add(1, new Vector2(0.15f, 1.21f));
            temp.Add(2, new Vector2(0.12f, 1.12f));
            temp.Add(3, new Vector2(0.12f, 1.4f));
            temp.Add(4, new Vector2(0.08f, 1.42f));
            temp.Add(5, new Vector2(0.08f, 1.42f));
            hairPivotFramesPerState.Add(AC.HurtLight, temp);

            // run
            temp = new Dictionary<int, Vector2>();
            temp.Add(0, new Vector2(-0.2f, 1.17f));
            temp.Add(1, new Vector2(-0.21f, 1.36f));
            temp.Add(2, new Vector2(-0.27f, 1.34f));
            temp.Add(3, new Vector2(-0.3f, 1.1f));
            temp.Add(4, new Vector2(-0.3f, 1.1f));
            temp.Add(5, new Vector2(-0.26f, 1.33f));
            temp.Add(6, new Vector2(-0.25f, 1.29f));
            temp.Add(7, new Vector2(-0.23f, 1.12f));
            hairPivotFramesPerState.Add(AC.Run, temp);

            // run start
            temp = new Dictionary<int, Vector2>();
            temp.Add(0, new Vector2(0.04f, 1.3f));
            temp.Add(1, new Vector2(-0.02f, 1.27f));
            temp.Add(2, new Vector2(-0.12f, 1.27f));
            hairPivotFramesPerState.Add(AC.RunStart, temp);

            // run stop
            temp = new Dictionary<int, Vector2>();
            temp.Add(0, new Vector2(0f, 1f));
            temp.Add(1, new Vector2(-0.02f, 1.08f));
            temp.Add(2, new Vector2(0f, 1.15f));
            temp.Add(3, new Vector2(-0.02f, 1.29f));
            temp.Add(4, new Vector2(-0.02f, 1.31f));
            hairPivotFramesPerState.Add(AC.RunStop, temp);

            // staff spellcast bolt down
            temp = new Dictionary<int, Vector2>();
            temp.Add(0, new Vector2(-0.18f, 1.32f));
            temp.Add(1, new Vector2(-0.1f, 1.32f));
            temp.Add(2, new Vector2(0f, 1.33f));
            temp.Add(3, new Vector2(-0.24f, 1.23f));
            temp.Add(4, new Vector2(-0.3f, 1.18f));
            temp.Add(5, new Vector2(-0.19f, 1.24f));
            temp.Add(6, new Vector2(-0.21f, 1.26f));
            hairPivotFramesPerState.Add(AC.SpellcastStaffBoltDown, temp);

            // staff spellcast bolt
            temp = new Dictionary<int, Vector2>();
            temp.Add(0, new Vector2(0.02f, 1.37f));
            temp.Add(1, new Vector2(0.02f, 1.37f));
            temp.Add(2, new Vector2(0.02f, 1.37f));
            temp.Add(3, new Vector2(-0.59f, 1.3f));
            temp.Add(4, new Vector2(-0.31f, 1.4f));
            temp.Add(5, new Vector2(-0.12f, 1.42f));
            temp.Add(6, new Vector2(0.04f, 1.36f));
            hairPivotFramesPerState.Add(AC.SpellcastStaffBolt, temp);

            // staff spellcast bolt up
            temp = new Dictionary<int, Vector2>();
            temp.Add(0, new Vector2(-0.13f, 1.37f));
            temp.Add(1, new Vector2(-0.31f, 1.38f));
            temp.Add(2, new Vector2(-0.31f, 1.38f));
            temp.Add(3, new Vector2(-0.37f, 1.25f));
            temp.Add(4, new Vector2(-0.28f, 1.33f));
            temp.Add(5, new Vector2(-0.15f, 1.35f));
            temp.Add(6, new Vector2(-0.12f, 1.36f));
            hairPivotFramesPerState.Add(AC.SpellcastStaffBoltUp, temp);

            // staff concentration
            temp = new Dictionary<int, Vector2>();
            temp.Add(0, new Vector2(-0.06f, 1.36f));
            hairPivotFramesPerState.Add(AC.StaffConcentration, temp);

            // wallslide
            temp = new Dictionary<int, Vector2>();
            temp.Add(0, new Vector2(-0.16f, 1.42f));
            hairPivotFramesPerState.Add(AC.Wallslide, temp);
        }

        private void PopulateCloakPivots()
        {
            //Idle state
            Dictionary<int, Vector2> temp = new Dictionary<int, Vector2>();
            temp.Add(0, new Vector2(-0.094f, 0.976f));
            cloakPivotFramesPerState.Add(AC.Idle, temp);

            //ascend
            temp = new Dictionary<int, Vector2>();
            temp.Add(0, new Vector2(-0.449f, 0.913f));
            cloakPivotFramesPerState.Add(AC.Ascend, temp);

            //descent
            temp = new Dictionary<int, Vector2>();
            temp.Add(0, new Vector2(-0.078f, 1.023f));
            cloakPivotFramesPerState.Add(AC.Descent, temp);

            // hurt_heavy
            temp = new Dictionary<int, Vector2>();
            temp.Add(0, new Vector2(-0.647f, 0.684f));
            temp.Add(1, new Vector2(-0.527f, 0.741f));
            temp.Add(2, new Vector2(-0.71f, 0.955f));
            temp.Add(3, new Vector2(-0.559f, 0.939f));
            temp.Add(4, new Vector2(-0.449f, 1.033f));
            temp.Add(5, new Vector2(-0.402f, 1.033f));
            temp.Add(6, new Vector2(-0.225f, 1.064f));
            temp.Add(7, new Vector2(-0.246f, 1.017f));
            temp.Add(8, new Vector2(-0.262f, 0.97f));
            temp.Add(9, new Vector2(-0.288f, 0.965f));
            temp.Add(10, new Vector2(-0.298f, 0.975f));
            cloakPivotFramesPerState.Add(AC.HurtHeavy, temp);

            // hurt_light
            temp = new Dictionary<int, Vector2>();
            temp.Add(0, new Vector2(-0.042f, 0.996f));
            temp.Add(1, new Vector2(-0.052f, 0.996f));
            temp.Add(2, new Vector2(-0.078f, 0.886f));
            temp.Add(3, new Vector2(-0.068f, 1.037f));
            temp.Add(4, new Vector2(-0.058f, 1.016f));
            temp.Add(5, new Vector2(-0.042f, 1.032f));
            cloakPivotFramesPerState.Add(AC.HurtLight, temp);

            // run
            temp = new Dictionary<int, Vector2>();
            temp.Add(0, new Vector2(-0.288f, 0.778f));
            temp.Add(1, new Vector2(-0.288f, 0.958f));
            temp.Add(2, new Vector2(-0.288f, 0.958f));
            temp.Add(3, new Vector2(-0.3f, 0.644f));
            temp.Add(4, new Vector2(-0.3f, 0.644f));
            temp.Add(5, new Vector2(-0.271f, 0.987f));
            temp.Add(6, new Vector2(-0.277f, 0.958f));
            temp.Add(7, new Vector2(-0.277f, 0.72f));
            cloakPivotFramesPerState.Add(AC.Run, temp);

            // run start
            temp = new Dictionary<int, Vector2>();
            temp.Add(0, new Vector2(-0.099f, 0.943f));
            temp.Add(1, new Vector2(-0.062f, 0.938f));
            temp.Add(2, new Vector2(-0.088f, 0.948f));
            cloakPivotFramesPerState.Add(AC.RunStart, temp);

            // run stop
            temp = new Dictionary<int, Vector2>();
            temp.Add(0, new Vector2(-0.072f, 0.724f));
            temp.Add(1, new Vector2(-0.109f, 0.734f));
            temp.Add(2, new Vector2(-0.072f, 0.833f));
            temp.Add(3, new Vector2(-0.051f, 0.99f));
            temp.Add(4, new Vector2(-0.067f, 0.996f));
            cloakPivotFramesPerState.Add(AC.RunStop, temp);

            // staff spellcast bolt down
            temp = new Dictionary<int, Vector2>();
            temp.Add(0, new Vector2(-0.067f, 1.048f));
            temp.Add(1, new Vector2(-0.025f, 1.038f));
            temp.Add(2, new Vector2(0.001f, 1.059f));
            temp.Add(3, new Vector2(-0.114f, 0.944f));
            temp.Add(4, new Vector2(-0.208f, 0.913f));
            temp.Add(5, new Vector2(-0.13f, 0.934f));
            temp.Add(6, new Vector2(-0.161f, 1.002f));
            cloakPivotFramesPerState.Add(AC.SpellcastStaffBoltDown, temp);

            // staff spellcast bolt
            temp = new Dictionary<int, Vector2>();
            temp.Add(0, new Vector2(-0.062f, 1.033f));
            temp.Add(1, new Vector2(-0.052f, 0.996f));
            temp.Add(2, new Vector2(-0.052f, 0.996f));
            temp.Add(3, new Vector2(-0.485f, 0.923f));
            temp.Add(4, new Vector2(-0.297f, 0.986f));
            temp.Add(5, new Vector2(-0.114f, 1.028f));
            temp.Add(6, new Vector2(0.016f, 1.002f));
            cloakPivotFramesPerState.Add(AC.SpellcastStaffBolt, temp);

            // staff spellcast bolt up
            temp = new Dictionary<int, Vector2>();
            temp.Add(0, new Vector2(-0.088f, 1.023f));
            temp.Add(1, new Vector2(-0.25f, 1.06f));
            temp.Add(2, new Vector2(-0.276f, 1.003f));
            temp.Add(3, new Vector2(-0.281f, 1.04f));
            temp.Add(4, new Vector2(-0.229f, 1.05f));
            temp.Add(5, new Vector2(-0.151f, 1.066f));
            temp.Add(6, new Vector2(-0.109f, 0.967f));
            cloakPivotFramesPerState.Add(AC.SpellcastStaffBoltUp, temp);

            // staff concentration
            temp = new Dictionary<int, Vector2>();
            temp.Add(0, new Vector2(-0.067f, 0.994f));
            cloakPivotFramesPerState.Add(AC.StaffConcentration, temp);

            // wallslide
            temp = new Dictionary<int, Vector2>();
            temp.Add(0, new Vector2(-0.171f, 1.109f));
            cloakPivotFramesPerState.Add(AC.Wallslide, temp);
        }

        // Animations that should not have the hinge stuff.
        private void SetIncorrectHingeStateList()
        {
            _incorrectStateList = new HashSet<int>();
            _incorrectStateList.Add(AC.DoubleJump);
            _incorrectStateList.Add(AC.Dash);
            _incorrectStateList.Add(AC.DashEnd);
            _incorrectStateList.Add(AC.StaffAttack1);
            _incorrectStateList.Add(AC.StaffAttack2);
            _incorrectStateList.Add(AC.StaffAttackAir1);
            _incorrectStateList.Add(AC.StaffAttackAir2);
            _incorrectStateList.Add(AC.StaffAttackAirDown1);
            _incorrectStateList.Add(AC.StaffAttackAirDown2);
            _incorrectStateList.Add(AC.StaffAttackAirUp);
            _incorrectStateList.Add(AC.SpellcastStaffHeavy);
            _incorrectStateList.Add(AC.SpellcastStaffAoE);
            
        }
    }
}