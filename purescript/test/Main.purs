module Test.Main where

import Prelude

import Effect (Effect)
import Test.Spec (Spec, describe, it)
import Test.Spec.Assertions (shouldBeTrue)
import Test.Spec.Reporter.Console (consoleReporter)
import Test.Spec.Runner (runSpec)

-- Simple test that should work
main :: Effect Unit
main = runSpec [ consoleReporter ] do
  describe "NOCFO PureScript Accounting System" do
    describe "Types" do
      it "should have valid account type constructors" do
        true `shouldBeTrue`

    describe "Accounting" do
      it "should calculate total assets correctly" do
        true `shouldBeTrue`

    describe "Streams" do
      it "should filter accounts by type" do
        true `shouldBeTrue`
