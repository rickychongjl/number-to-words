# Number to words converter app

## Table of Contents
- [Overview](#overview)
- [To interact with the app](#to-interact-with-the-app)
- [Validation rules](#validation-rules)
- [Project Structure](#project-structure)
- [Pre-requisites](#pre-requisites)
- [To run this locally](#to-run-this-locally)
- [How to host the app](#how-to-host-the-app)
- [To run the unit tests locally](#to-run-the-unit-tests-locally)
- [Docos](#docos)

### Overview
1. This is a simple number to words converter app.
2. It contains a simple web UI to allow you to interactively put in numbers and the app will return you the word format of it.

> This app follows the Australian (British) English conventions, short scale

### To interact with the app
To interact with the app, you can either [run it locally](#to-run-this-locally) or access the live site at
```
https://number-to-words-avbta9c0amambmay.australiaeast-01.azurewebsites.net/
```
Upon having the site up, you should see the home page, and there will be a text box with a label "Enter an amount" and a button called "Convert".

Put in any number
- `123.45` and it will return `ONE HUNDRED AND TWENTY-THREE DOLLARS AND FORTY-FIVE CENTS`

> ❗Amounts are rounded to 2 decimal places, half up (`0.005 -> ONE CENT`)

There are certain validation rules that the input must adhere to, and upon submitting any invalid input, the app will return you the validation errors accordingly. Read the validation rules [here](#validation-rules)

### Validation rules
Below input types are rejected
1. Empty or whitespace values
2. Input with positive (+) or negative (-) signs
3. Input with a decimal point that has no digits after it
    - For eg: `5.`
4. More than 1 decimal points
5. Incorrect comma placement before decimal point, commas can only be placed after every third digit from right to left excluding decimal point.
6. Commas included after decimal points
7. Input contains non-numerical values, and symbols/special characters excluding commas, and dots.
8. Input of 1 septillion (1,000,000,000,000,000,000,000,000) or more, after rounding to 2 decimal places.

### Project structure
```
.
└── number-to-words/
    ├── docs/
    │   ├── solution-design.md
    │   ├── test-plan.md
    │   └── scratch-pad.md
    ├── src/
    │   ├── NumberToWords.Core/
    │   │   └── [Core conversion logic and validation methods]
    │   └── NumberToWords.Web/
    │       └── [Controller and basic HTML]
    └── tests/
        └── NumberToWords.Tests/
            └── [Unit tests]
```

### Pre-requisites
To run or test this locally, you need to ensure `.NET 10 SDK` and `Git` are installed.

### To run this locally
If this is your first time running this, ensure you have cloned repo on your local.

To clone the repo
```
git clone https://github.com/rickychongjl/number-to-words.git
```

To run the app locally

From the root
```
cd number-to-words
dotnet build
dotnet run --project src/NumberToWords.Web
```

You can now access the app using `http://localhost:5027`

### How to host the app
From the repo root, run
```
dotnet publish src/NumberToWords.Web -c Release -o ./publish
cd publish
```
You should see a working executable `NumberToWords.Web.exe`.

You can run the app directly by running the executable and it listens on `http://localhost:5000`, or you can host it in any ASP.NET Core host, like Azure App Service, which is what I am using.

CI/CD has been configured for this app, every push to `main` branch triggers Github Action Workflow, which builds, runs the unit tests, and deploys to an Azure App Service.

The YML can be seen [here](/.github/workflows/main_number-to-words.yml)

### To run the unit tests locally

Ensure you have cloned the repo, if not look into the previous step on how to clone the repo

From the repo root
```
dotnet test
```
### Docos
Access to the following docos
- [Solution Design](/docs/solution-design.md)
- [Test Plan](/docs/test-plan.md)
- [Scratch Pad](/docs/scratch-pad.md)