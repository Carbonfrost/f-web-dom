.PHONY: dotnet/build dotnet/test dotnet/publish dotnet/push dotnet/generate

CONFIGURATION ?= Release

## Generate generated code
dotnet/generate:
	srgen -c Carbonfrost.Commons.Web.Dom.Resources.SR \
		-r Carbonfrost.Commons.Web.Dom.Automation.SR \
		--resx \
		dotnet/src/Carbonfrost.Commons.Web.Dom/Automation/SR.properties
	/bin/sh -c "t4 dotnet/src/Carbonfrost.Commons.Web.Dom/Automation/TextTemplates/Fluent.tt -o dotnet/src/Carbonfrost.Commons.Web.Dom/Automation/TextTemplates/Fluent.g.cs"

## Execute dotnet unit tests
dotnet/test: dotnet/publish -dotnet/test

-dotnet/test:
	fspec -i dotnet/test/Carbonfrost.UnitTests.Web.Dom/Content \
		dotnet/test/Carbonfrost.UnitTests.Web.Dom/bin/$(CONFIGURATION)/netcoreapp3.0/publish/Carbonfrost.Commons.Core.dll \
		dotnet/test/Carbonfrost.UnitTests.Web.Dom/bin/$(CONFIGURATION)/netcoreapp3.0/publish/Carbonfrost.Commons.Core.Runtime.Expressions.dll \
		dotnet/test/Carbonfrost.UnitTests.Web.Dom/bin/$(CONFIGURATION)/netcoreapp3.0/publish/Carbonfrost.Commons.Web.Dom.dll \
		dotnet/test/Carbonfrost.UnitTests.Web.Dom/bin/$(CONFIGURATION)/netcoreapp3.0/publish/Carbonfrost.UnitTests.Web.Dom.dll

## Run unit tests with code coverage
dotnet/cover: dotnet/publish -check-command-coverlet
	coverlet \
		--target "make" \
		--targetargs "-- -dotnet/test" \
		--format cobertura \
		--exclude-by-attribute 'Obsolete' \
		--exclude-by-attribute 'GeneratedCode' \
		--exclude-by-attribute 'CompilerGenerated' \
		dotnet/test/Carbonfrost.UnitTests.Web.Dom/bin/$(CONFIGURATION)/netcoreapp3.0/publish/Carbonfrost.UnitTests.Web.Dom.dll

include eng/.mk/*.mk
