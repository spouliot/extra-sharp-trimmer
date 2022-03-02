
## Extra Steps

### RemoveBuggyResourcesStep

This step is a (hopefully) temporary workaround for the following issues:
* https://github.com/xamarin/xamarin-macios/issues/14257
* https://github.com/dotnet/linker/issues/2661

This affects most of net6 iOS/tvOS/macOS applications since they all uses
the new `NFloat` type and might not use anything else from the
`System.Runtime.InteropServices.dll` assembly. Note that this situation
could also happen on other assemblies as well.

To enable this step inside your project you need to add the following
snippet inside your `.csproj` file:

```
<ItemGroup>
  <_TrimmerCustomSteps Include="/full/path/to/the/extra-sharp-trimmer.dll" BeforeStep="OutputStep" Type="ExtraSteps.RemoveBuggyResourcesStep" />
</ItemGroup>
```

This will run the `RemoveBuggyResourcesStep` step before the `OutputStep`
step, giving it a chance to remove the extra, non-required resources from
the assemblies.

The removal of the resources saves 3KB for `System.Runtime.InteropServices.dll`
which goes from 7.5 KB to 4.5 KB. That's still a lot for a single type-forwarder
but that's sadly the minimal price to include another assembly inside an app.
