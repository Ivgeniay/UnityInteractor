using NodeEngine.DialogueType;
using System.Collections.Generic;
using System;

namespace NodeEngine.Utilities
{
    public static class DSConstants
    {
        public const string ACTION_PN = "Action";
        public const string REFERENCE_PN = "ReferenceAction";
        public const string PARALLEL_PN = "ParallelAction";
        public const string NEXT_PN = "NextAction";

        public static Type[] AvalilableTypes { get; private set; }
        public static Type[] NumberTypes { get; private set; }
        public static Type[] PrimitiveTypes { get; private set; }
        public static Type[] DialogueTypes { get; private set; }
        public static Type[] CollectionsTypes { get; private set; }
        public static Type[] TypeTypes { get; private set; } 
        public static Type[] AllTypes { get; private set; } 

        public static string DEFAULT_ASSEMBLY = "Assembly-CSharp"; 

        public readonly static string All;
        public readonly static string Number;
        public readonly static string Dialogue;
        public readonly static string Int;
        public readonly static string String;
        public readonly static string Float;
        public readonly static string Double;
        public readonly static string Bool;

        static DSConstants()
        {
            AvalilableTypes = new Type[]
            {
                typeof(string),
                typeof(int),
                typeof(Int16),
                typeof(Int32),
                typeof(Int64),
                typeof(float),
                typeof(Single),
                typeof(double),
                typeof(bool),
                typeof(Dialogue),
                typeof(DialogueDisposer.DSDialogueOption.DSDialogue)
            };

            PrimitiveTypes = new Type[]
            {
                typeof(string),
                typeof(int),
                typeof(Int16),
                typeof(Int32),
                typeof(Int64),
                typeof(float),
                typeof(Single),
                typeof(double),
                typeof(bool),
            };

            NumberTypes = new Type[]
            {
                typeof(int),
                typeof(Int16),
                typeof(Int32),
                typeof(Int64),
                typeof(float),
                typeof(Single),
                typeof(double),
            };

            DialogueTypes = new Type[] { typeof(Dialogue), typeof(DialogueDisposer.DSDialogueOption.DSDialogue) };
            TypeTypes = new Type[] { typeof(Type) };
            AllTypes = new Type[] { typeof(AllTypes) };

            CollectionsTypes = new Type[]
            {
                typeof(List<object>),
                typeof(Dictionary<object,object>)
            };

            All = "All";
            Int = typeof(int).Name;
            String = typeof(string).Name;
            Float = typeof(float).Name;
            Double = typeof(double).Name;
            Bool = typeof(bool).Name; 
            Number = "Number";
            Dialogue = "Dialogue";
        }
    }
}