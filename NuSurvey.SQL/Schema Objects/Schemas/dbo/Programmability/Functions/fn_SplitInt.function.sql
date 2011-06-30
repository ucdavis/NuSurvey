CREATE FUNCTION [dbo].[fn_SplitInt]
(
    @List       nvarchar(4000),
    @Delimiter  char(1)= ','
)
RETURNS @Values TABLE
(
    Position int IDENTITY PRIMARY KEY,
    Number int
)

AS

  BEGIN

  -- set up working variables
  DECLARE @Index INT
  DECLARE @ItemValue nvarchar(100)
  SELECT @Index = 1 

  -- iterate until we have no more characters to work with
  WHILE @Index > 0

    BEGIN

      -- find first delimiter
      SELECT @Index = CHARINDEX(@Delimiter,@List)

      -- extract the item value
      IF @Index  > 0     -- if found, take the value left of the delimiter
        SELECT @ItemValue = LEFT(@List,@Index - 1)
      ELSE               -- if none, take the remainder as the last value
        SELECT @ItemValue = @List

      -- insert the value into our new table
      INSERT INTO @Values (Number) VALUES (CAST(@ItemValue AS int))

      -- remove the found item from the working list
      SELECT @List = RIGHT(@List,LEN(@List) - @Index)

      -- if list is empty, we are done
      IF LEN(@List) = 0 BREAK

    END

  RETURN

  END