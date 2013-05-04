using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content.Pipeline;
using Microsoft.Xna.Framework.Content.Pipeline.Graphics;
using Microsoft.Xna.Framework.Content.Pipeline.Processors;
using Microsoft.Xna.Framework.Content.Pipeline.Serialization.Compiler;
using GameLibrary;

// TODO: replace this with the type you want to write out.
using TWrite = System.String;

namespace XMLContentExtension
{
    /// <summary>
    /// This class will be instantiated by the XNA Framework Content Pipeline
    /// to write the specified data type into binary .xnb format.
    ///
    /// This should be part of a Content Pipeline Extension Library project.
    /// </summary>
    [ContentTypeWriter]
    public class CAnimationHandlerContentWriter : ContentTypeWriter<CAnimationHandler>
    {
        protected override void Write(ContentWriter output, CAnimationHandler value)
        {
            output.Write(value.ID);
            output.Write(value.Name);
            output.Write(value.Loop);
            output.Write(value.ReverseOnEnd);
            output.WriteObject<List<string>>(value.Sprites);
        }

        public override string GetRuntimeReader(TargetPlatform targetPlatform)
        {
            // TODO: change this to the name of your ContentTypeReader
            // class which will be used to load this data.
            return typeof(CAnimationContentReader).AssemblyQualifiedName;
        }
    }
}
