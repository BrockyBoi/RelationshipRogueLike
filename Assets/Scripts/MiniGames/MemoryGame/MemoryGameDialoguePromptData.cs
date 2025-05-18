using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MemoryGame.Dialogue
{
    [CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/MemoryGame/Dialogue Prompt", order = 1)]
    public class MemoryGameDialoguePromptData : ScriptableObject
    {
        public EMemoryType MemoryType;
        public Sprite MemorySprite;
        public List<string> QuestionPrompts;
        public List<string> CorrectAnswers;
        public List<string> IncorrectAnswers;
    }
}
