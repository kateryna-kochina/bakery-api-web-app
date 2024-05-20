add-migration:
	dotnet ef migrations add $(name) --output-dir Data/Migrations

remove-migration:
	dotnet ef migrations remove

apply-migrations:
	dotnet ef database update

run:
	dotnet run