using System;
using UnityEngine;

namespace MemoryGame.Dialogue
{
    public class MemoryGameDialoguePromptsManager : MonoBehaviour
    {
        public static MemoryGameDialoguePromptsManager Instance { get; private set; }

        [SerializeField]
        private MemoryGameDialoguePromptsDictionary _prompts;

        private void Awake()
        {
            Instance = this;
        }

        public MemoryGameDialoguePromptData GetMemoryGameDialoguePromptData(EMemoryType type)
        {
            bool containsKey = _prompts.ContainsKey(type);
            return _prompts.ContainsKey(type) ? _prompts[type] : default(MemoryGameDialoguePromptData);
        }
    }

    [Serializable]
    public class MemoryGameDialoguePromptsDictionary : UnitySerializedDictionary<EMemoryType, MemoryGameDialoguePromptData> { }
}
