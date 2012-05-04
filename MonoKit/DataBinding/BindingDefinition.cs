namespace MonoKit.DataBinding
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Defines a binding between two properties
    /// </summary>
    public class BindingDefinition
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MonoKit.DataBinding.BindingDefinition"/> class.
        /// </summary>
        /// <param name='property'>
        /// The name of the property to create a binding definition for.  The target and source properties are assumed to be the
        /// same name.
        /// </param>
        public BindingDefinition(string property)
        {
            this.PropertyName = property;
            this.Binding = new Binding(property);
        }
        
        /// <summary>
        /// Initializes a new instance of the <see cref="MonoKit.DataBinding.BindingDefinition"/> class.
        /// </summary>
        /// <param name='targetProperty'>
        /// The name of the target property to bind to.
        /// </param>
        /// <param name='sourceProperty'>
        /// The name of the source property to bind to.
        /// </param>
        public BindingDefinition(string targetProperty, string sourceProperty)
        {
            this.PropertyName = targetProperty;
            this.Binding = new Binding(sourceProperty);
        }
        
        /// <summary>
        /// Initializes a new instance of the <see cref="MonoKit.DataBinding.BindingDefinition"/> class.
        /// </summary>
        /// <param name='targetProperty'>
        /// The name of the target property to bind to.
        /// </param>
        /// <param name='binding'>
        /// The source binding.
        /// </param>
        public BindingDefinition(string targetProperty, Binding binding)
        {
            this.PropertyName = targetProperty;
            this.Binding = binding;
        }
        
        /// <summary>
        /// Gets the name of the target property.
        /// </summary>
        /// <value>
        /// The name of the property.
        /// </value>
        public string PropertyName
        {
            get;
            private set;
        }
        
        /// <summary>
        /// Gets the source binding.
        /// </summary>
        /// <value>
        /// The binding.
        /// </value>
        public Binding Binding
        {
            get;
            private set;
        }
    }
}

