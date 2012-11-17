using System;

namespace MonoKit.Core.UnitTests
{
    public class Bootstrap
    {
    }

    // todo: we can have a PropertyAccessor that is specific to one thread as well, 
    // this can be useful in both regular and one-time binds to UI objects, we can call bind from the background
    // and still have the UI update - might still have issues for UILabels etc


    // todo: write tests for GetPropertyInfo for nested properties - ReflectionPropertyAccessor
    
    // todo: add support for binding to attached properties - should be fast compared to SetValue / GetValue thru reflection
    // - we can use binding assistants for this - the property name is the injected property name, and the assistant 
    // uses casts and injected properties to set / get the value
    // we'll need a unit test for that
    //
    // for this to work, we need to have IProprtyAccessor know about the property type of the injected property, so that converters can use it
    // we also need to allow binding to other injected properties

}

