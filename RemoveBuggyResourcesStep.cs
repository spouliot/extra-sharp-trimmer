using Mono.Cecil;
using Mono.Linker;
using Mono.Linker.Steps;

namespace ExtraSteps {

	public class RemoveBuggyResourcesStep : BaseStep {
	
        protected override void ProcessAssembly(AssemblyDefinition assembly)
		{
			if (Annotations.GetAction (assembly) != AssemblyAction.Link)
				return;

			var module = assembly.MainModule;
			if (!module.HasResources)
				return;
			
			var resources = module.Resources;
			for (int i = 0; i < resources.Count; i++) {
				var resource = resources [i];

				if (resource is not EmbeddedResource)
					continue;

				var name = resource.Name;
				switch (name) {
				case "ILLink.LinkAttributes.xml":
				case "ILLink.Substitutions.xml":
					// https://github.com/xamarin/xamarin-macios/issues/14257
					Warn (name);
					resources.RemoveAt (i--);
					break;
				case "FxResources.System.Runtime.InteropServices.SR.resources":
					// due to not processing `ILLink.Substitutions.xml` (see above) the resource is not removed
					if (assembly.Name.Name == "System.Runtime.InteropServices") {
						Warn (name);
						resources.RemoveAt (i--);
					}
					break;
				}
			}
		}

		void Warn (string resourceName)
		{
			var warning = MessageContainer.CreateCustomWarningMessage (Context, $"Resource {resourceName} should not be present inside a linked assembly at this stage.", 6001, default, WarnVersion.Latest);
			Context.LogMessage (warning);
		}
	}
}
