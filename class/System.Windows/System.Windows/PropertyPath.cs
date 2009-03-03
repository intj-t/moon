//
// PropertyPath.cs
//
// Contact:
//   Moonlight List (moonlight-list@lists.ximian.com)
//
// Copyright 2008 Novell, Inc.
//
// Permission is hereby granted, free of charge, to any person obtaining
// a copy of this software and associated documentation files (the
// "Software"), to deal in the Software without restriction, including
// without limitation the rights to use, copy, modify, merge, publish,
// distribute, sublicense, and/or sell copies of the Software, and to
// permit persons to whom the Software is furnished to do so, subject to
// the following conditions:
// 
// The above copyright notice and this permission notice shall be
// included in all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
// MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
// NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE
// LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION
// OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION
// WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
//

using System;
using System.Collections.Generic;

namespace System.Windows
{
	public sealed class PropertyPath
	{
		private string path;
		private object [] path_parameters;
		private DependencyProperty property;
		
		public PropertyPath (string path, params object [] pathParameters)
		{
			if (path == null)
				throw new ArgumentOutOfRangeException ("path");
			
			this.path = path;
			this.path_parameters = pathParameters;
		}

		public PropertyPath (object parameter)
		{
			if (parameter is DependencyProperty) {
				//DependencyProperty property = parameter as DependencyProperty;
				//
				//if (property.IsAttached)
				//	path = "(" + property.DeclaringType.Name + "." + property.Name + ")";
				//else
				//	path = property.Name;
				
				// Silverlight always sets "(0)" as the path no matter what DependencyProperty was passed in...
				path = "(0)";
				property = (DependencyProperty) parameter;
			} else if (parameter is string) {
				path = parameter as string;
			} else {
				path = null;
			}
		}
		
		internal IntPtr NativeDP {
			get { return property == null ? IntPtr.Zero : property.Native;  }
		}
		public string Path {
			get { return path; }
		}

		internal IList<object> PathParameters {
			get { return path_parameters; }
		}
	}
}
