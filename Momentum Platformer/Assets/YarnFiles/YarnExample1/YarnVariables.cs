namespace YVars {

    using Yarn.Unity;

    [System.CodeDom.Compiler.GeneratedCode("YarnSpinner", "3.1.4.0")]
    public partial class YarnVariables : Yarn.Unity.InMemoryVariableStorage, Yarn.Unity.IGeneratedVariableStorage {
        // Accessor for Number $gold
        public float Gold {
            get => this.GetValueOrDefault<float>("$gold");
            set => this.SetValue<float>("$gold", value);
        }

    }
}
