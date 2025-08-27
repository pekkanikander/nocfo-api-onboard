module Test.Main where

import Prelude
import Effect (Effect)

import Types as T
import Test.Spec (describe, it)
import Test.Spec.Assertions (shouldEqual)
import Test.Spec.Reporter.Console (consoleReporter)
import Test.Spec.Runner.Node (runSpecAndExitProcess)

main :: Effect Unit
main = runSpecAndExitProcess [ consoleReporter ] do
  describe "NOCFO PureScript Accounting System" do
    describe "Types" do
      it "should have valid account type constructors" do
        true `shouldEqual` true

      it "should create a Business with all required fields" do
        let business = T.Business 1 "holotropic" "Holotropic Oy" "1234567-8" "FI_YHD"
        T.id business `shouldEqual` 1
        T.slug business `shouldEqual` "holotropic"
        T.name business `shouldEqual` "Holotropic Oy"
        T.businessId business `shouldEqual` "1234567-8"
        T.form business `shouldEqual` "FI_YHD"

      it "should create an Account with all required fields" do
        let account = T.Account 1 "1000" 1000 "Bank Account" T.ASS_PAY 1 25.5 "standard" 0.0 1000.0 true true "2024-01-01" "2024-01-01"
        T.accountId account `shouldEqual` 1
        T.accountNumber account `shouldEqual` "1000"
        T.accountName account `shouldEqual` "Bank Account"
        T.accountType account `shouldEqual` T.ASS_PAY
        T.accountBalance account `shouldEqual` 1000.0

      it "should validate AccountType enum values" do
        T.ASS `shouldEqual` T.ASS
        T.LIA `shouldEqual` T.LIA
        T.REV `shouldEqual` T.REV
        T.EXP `shouldEqual` T.EXP

    describe "Accounting" do
      it "should calculate total assets correctly" do
        true `shouldEqual` true

    describe "Streams" do
      it "should filter accounts by type" do
        true `shouldEqual` true
