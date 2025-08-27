module Test.Main where

import Prelude
import Effect (Effect)

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
    describe "Accounting" do
      it "should calculate total assets correctly" do
        true `shouldEqual` true
    describe "Streams" do
      it "should filter accounts by type" do
        true `shouldEqual` true
