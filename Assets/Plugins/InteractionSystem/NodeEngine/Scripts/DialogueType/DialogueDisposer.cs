using System.Collections.Generic;
using UnityEngine;
using System;
using static NodeEngine.DialogueDisposer.DSDialogueOption;

namespace NodeEngine
{
    [System.Serializable]
    public class DialogueDisposer
    {
        public static void TestDialogue(DSDialogue startDialogue)
        {
            Debug.Log("Dialogue text: " + startDialogue.Text);
            var opt = startDialogue.GetOptions();
            foreach (var op in opt) Option(op);
        }
        public static void Option(DSDialogueOption option)
        {
            Debug.Log("Options text: " + option.Text);
            if (option.NextDialogue != null) TestDialogue(option.NextDialogue);
        }

        [System.Serializable]
        public record DSDialogueOption
        {
            public string Text { get; private set; }
            public DSDialogue NextDialogue { get; private set; }
            private Func<bool> Func { get; set; }

            public DSDialogueOption(string text, DSDialogue nextDialogues = null, Func<bool> func = null)
            {
                this.Text = text;
                this.NextDialogue = nextDialogues;
                Func = func == null ? () => true : func;
            }

            [System.Serializable]
            public class DSDialogue
            {
                #region Fields
                [SerializeField] public System.String Text;
                public List<DSDialogueOption> DSDialogueOption = new();
                #endregion

                public IEnumerable<DSDialogueOption> GetOptions()
                {
                    foreach (DSDialogueOption option in DSDialogueOption)
                        if (option.Func()) yield return option;
                }
            }
        }
    }
}
