.PHONY: dotnet/test dotnet/generate

-include eng/.mk/*.mk

## Generate generated code
dotnet/generate:
	$(Q) srgen -c Carbonfrost.Commons.Web.Dom.Resources.SR \
		-r Carbonfrost.Commons.Web.Dom.Automation.SR \
		--resx \
		dotnet/src/Carbonfrost.Commons.Web.Dom/Automation/SR.properties
	$(Q) dotnet t4 dotnet/src/Carbonfrost.Commons.Web.Dom/Automation/TextTemplates/Fluent.tt -o dotnet/src/Carbonfrost.Commons.Web.Dom/Automation/TextTemplates/Fluent.g.cs

## Execute dotnet unit tests
dotnet/test: dotnet/publish -dotnet/test

-dotnet/test:
	$(Q) fspec $(FSPEC_OPTIONS) -i dotnet/test/Carbonfrost.UnitTests.Web.Dom/Content \
		dotnet/test/Carbonfrost.UnitTests.Web.Dom/bin/$(CONFIGURATION)/netcoreapp3.0/publish/Carbonfrost.UnitTests.Web.Dom.dll

## Run unit tests with code coverage
dotnet/cover: dotnet/publish -check-command-coverlet
	$(Q) coverlet \
		--target "make" \
		--targetargs "-- -dotnet/test" \
		--format lcov \
		--output lcov.info \
		--exclude-by-attribute 'Obsolete' \
		--exclude-by-attribute 'GeneratedCode' \
		--exclude-by-attribute 'CompilerGenerated' \
		dotnet/test/Carbonfrost.UnitTests.Web.Dom/bin/$(CONFIGURATION)/netcoreapp3.0/publish/Carbonfrost.UnitTests.Web.Dom.dll

