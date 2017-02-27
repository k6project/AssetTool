using System;
using OpenGL;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AssetTool
{

    public class Parameter
    {
        public virtual void Set(Matrix4x4 v) { throw new NotImplementedException(); }
        public virtual void Set(Vertex4f v) { throw new NotImplementedException(); }
        public virtual void Set(Vertex3f v) { throw new NotImplementedException(); }
        public virtual void Assign(int handle) { throw new NotImplementedException(); }
    }

    public class TypedParameter<T> : Parameter
    {
        protected T Value;
        protected bool Changed = false;

        protected virtual void Update(int handle) { throw new NotImplementedException(); }

        protected void SetValue(T val)
        {
            Value = val;
            Changed = true;
        }

        public override void Assign(int handle)
        {
            if (Changed && handle >= 0)
            {
                Update(handle);
                Changed = false;
            }
        }
    }

    public class ParameterMatrix4x4 : TypedParameter<Matrix4x4>
    {
        public override void Set(Matrix4x4 v) { SetValue(v); }
        protected override void Update(int handle) { Gl.UniformMatrix4(handle, 1, false, Value.ToArray()); }
    }

    public class ParameterVector4 : TypedParameter<Vertex4f>
    {
        public override void Set(Vertex4f v) { SetValue(v); }
        protected override void Update(int handle) { Gl.Uniform4(handle, Value.X, Value.Y, Value.Z, Value.W); }
    }

    public class ParameterVector3 : TypedParameter<Vertex3f>
    {
        public override void Set(Vertex3f v) { SetValue(v); }
        protected override void Update(int handle) { Gl.Uniform3(handle, Value.X, Value.Y, Value.Z); }
    }


}
