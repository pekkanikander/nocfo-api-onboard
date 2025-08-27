module Types where

-- Business type definition for NOCFO API
-- Minimal business info: ID for display + slug for account fetching

data Business = Business Int String String String String

-- Field accessors
id :: Business -> Int
id (Business i _ _ _ _) = i

slug :: Business -> String
slug (Business _ s _ _ _) = s

name :: Business -> String
name (Business _ _ n _ _) = n

businessId :: Business -> String
businessId (Business _ _ _ bi _) = bi

form :: Business -> String
form (Business _ _ _ _ f) = f
