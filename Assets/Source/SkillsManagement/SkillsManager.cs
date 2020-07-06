using Assets.Source.GameManagement;
using UnityEngine;

namespace Assets.Source.SkillsManagement
{
    /// <summary>
    /// Controls players skills
    /// </summary>
    public class SkillsManager : MonoBehaviour
    {
        private static SkillsManager instance;

        public static SkillsManager Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = FindObjectOfType<SkillsManager>();
                }
                return instance;
            }
        }

        /// <summary>
        /// Try to send request to use left skill
        /// </summary>
        public void UseLeftSkill()
        {
            GameManager.Instance.OnPlayerUseSkill(0);
        }

        /// <summary>
        /// Try to send request to use right skill
        /// </summary>
        public void UseRightSkill()
        {
            GameManager.Instance.OnPlayerUseSkill(1);
        }
    }
}
