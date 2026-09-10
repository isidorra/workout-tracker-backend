SHELL := /bin/bash

SOLUTION    := WorkoutTracker.slnx
API_PROJECT := src/WorkoutTracker.Api
DB_PROJECT  := src/WorkoutTracker.Infrastructure
CONFIG      ?= Debug

ENV_PORT    := $(shell sed -n 's|.*localhost:\([0-9]*\).*|\1|p' .env 2>/dev/null | head -1)
PORT        ?= $(if $(ENV_PORT),$(ENV_PORT),8080)

EF_ARGS     := --project $(DB_PROJECT) --startup-project $(API_PROJECT)
MIG_DIR     := Persistence/Migrations
SQL_OUT     ?= artifacts/migration.sql

CYAN   := \033[36m
GREEN  := \033[32m
YELLOW := \033[33m
RED    := \033[31m
DIM    := \033[2m
BOLD   := \033[1m
RESET  := \033[0m

.DEFAULT_GOAL := help
.PHONY: help setup env restore build rebuild run watch test migration migration-remove sql format format-check clean ci info outdated

help: ## 📖  Show this help
	@printf "\n  $(BOLD)$(CYAN)WorkoutTracker$(RESET) $(DIM)· .NET 10 web API$(RESET)\n\n"
	@awk 'BEGIN {FS = ":.*?## "} /^[a-zA-Z_-]+:.*?## /{printf "  $(GREEN)%-13s$(RESET) %s\n", $$1, $$2}' $(MAKEFILE_LIST)
	@printf "\n  $(DIM)Overrides: make run PORT=9000 CONFIG=Release$(RESET)\n\n"

setup: env restore build ## 🌱  First-time setup: env + restore + build
	@printf "$(GREEN)🌱  Ready. Run 'make run' to start.$(RESET)\n"

env: ## 🔐  Create .env from .env.example
	@if [ -f .env ]; then \
		printf "$(YELLOW)⚠️   .env already exists — leaving it alone$(RESET)\n"; \
	else \
		cp .env.example .env; \
		printf "$(GREEN)✅  Created .env from .env.example$(RESET)\n"; \
	fi

restore: ## 📦  Restore NuGet packages + local tools
	@printf "$(CYAN)📦  Restoring packages…$(RESET)\n"
	@dotnet restore $(SOLUTION)
	@if [ -f .config/dotnet-tools.json ]; then dotnet tool restore; fi
	@printf "$(GREEN)✅  Restore complete$(RESET)\n"

build: ## 🔨  Build the solution
	@printf "$(CYAN)🔨  Building ($(CONFIG))…$(RESET)\n"
	@dotnet build $(SOLUTION) -c $(CONFIG) --nologo
	@printf "$(GREEN)✅  Build succeeded$(RESET)\n"

rebuild: clean build ## ♻️   Clean, then build from scratch

run: ## 🚀  Run the API
	@printf "$(CYAN)🚀  Starting API → $(BOLD)http://localhost:$(PORT)$(RESET)\n"
	@ASPNETCORE_URLS=http://localhost:$(PORT) dotnet run --project $(API_PROJECT) -c $(CONFIG)

watch: ## 👀  Run the API with hot reload
	@printf "$(CYAN)👀  Watching → $(BOLD)http://localhost:$(PORT)$(RESET)\n"
	@ASPNETCORE_URLS=http://localhost:$(PORT) dotnet watch --project $(API_PROJECT) run

test: ## 🧪  Run tests
	@if [ -z "$$(find tests -name '*.csproj' 2>/dev/null)" ]; then \
		printf "$(YELLOW)🧪  No test projects yet — skipping$(RESET)\n"; \
	else \
		printf "$(CYAN)🧪  Running tests…$(RESET)\n"; \
		dotnet test $(SOLUTION) -c $(CONFIG) --nologo; \
		printf "$(GREEN)✅  Tests passed$(RESET)\n"; \
	fi

migration: ## 🧬  Create a migration: make migration name=AddWorkouts
	@if [ -z "$(name)" ]; then \
		printf "$(RED)❌  Usage: make migration name=AddWorkouts$(RESET)\n"; \
		exit 1; \
	fi
	@printf "$(CYAN)🧬  Creating migration $(BOLD)$(name)$(RESET)$(CYAN)…$(RESET)\n"
	@dotnet ef migrations add $(name) $(EF_ARGS) --output-dir $(MIG_DIR)
	@printf "$(GREEN)✅  Migration created in $(DB_PROJECT)/$(MIG_DIR) — run 'make sql' next$(RESET)\n"

migration-remove: ## ↩️   Delete the most recent migration
	@printf "$(YELLOW)↩️   Removing the last migration…$(RESET)\n"
	@dotnet ef migrations remove $(EF_ARGS)
	@printf "$(GREEN)✅  Removed$(RESET)\n"

sql: ## 📜  Generate idempotent SQL for the migrations
	@printf "$(CYAN)📜  Scripting migrations → $(BOLD)$(SQL_OUT)$(RESET)\n"
	@mkdir -p $(dir $(SQL_OUT))
	@dotnet ef migrations script --idempotent --output $(SQL_OUT) $(EF_ARGS)
	@printf "$(GREEN)✅  Written to $(SQL_OUT)$(RESET) $(DIM)(gitignored — apply it by hand, then delete it)$(RESET)\n"

format: ## 🎨  Format code in place
	@printf "$(CYAN)🎨  Formatting…$(RESET)\n"
	@dotnet format $(SOLUTION)
	@printf "$(GREEN)✅  Formatted$(RESET)\n"

format-check: ## 🔎  Verify formatting without changing files
	@printf "$(CYAN)🔎  Verifying formatting…$(RESET)\n"
	@if dotnet format $(SOLUTION) --verify-no-changes; then \
		printf "$(GREEN)✅  Formatting is clean$(RESET)\n"; \
	else \
		printf "$(RED)❌  Formatting issues found — run 'make format'$(RESET)\n"; \
		exit 1; \
	fi

clean: ## 🧹  Remove build output
	@printf "$(YELLOW)🧹  Cleaning bin/ and obj/…$(RESET)\n"
	@find . -type d \( -name bin -o -name obj \) -prune -exec rm -rf {} + 2>/dev/null || true
	@printf "$(GREEN)✅  Clean$(RESET)\n"

ci: ## 🤖  Release build, same as GitHub Actions
	@printf "$(CYAN)🤖  CI build (Release)…$(RESET)\n"
	@dotnet restore $(SOLUTION)
	@dotnet build $(SOLUTION) --no-restore -c Release --nologo
	@printf "$(GREEN)✅  CI build succeeded$(RESET)\n"

info: ## ℹ️   Show SDK version and projects
	@printf "$(CYAN)ℹ️   SDK$(RESET)      $$(dotnet --version)\n"
	@printf "$(CYAN)ℹ️   Config$(RESET)   $(CONFIG) · port $(PORT)\n"
	@printf "$(CYAN)ℹ️   Projects$(RESET)\n"
	@dotnet sln $(SOLUTION) list | grep -F .csproj | sed 's|^|      |'

outdated: ## 🔍  List outdated NuGet packages
	@printf "$(CYAN)🔍  Checking for outdated packages…$(RESET)\n"
	@dotnet list $(SOLUTION) package --outdated
