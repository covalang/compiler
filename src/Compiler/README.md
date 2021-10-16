# Cova Programming Language



arithmetic


relation
```
== equality
!= inequality
> greater-than
< less-than
>= greater-than-or-equal
<= less-than-or-equal
>< within
<> without

'e' >< "hello" // true
'z' <> "hello" // true

"abc" \/ "bcd" // "abcd"
"abc" >< "bcd" // "bc"
"abc" <> "bcd" // "ad"

scope (start, continue, end) vs statement (delimit on same line)

Dented	\n\t{!n+1}	\n\t{n}	\n\t{n-1}	,
Braced	{	;	}	;

local nums = [1..3]
nums map + ',' // ["1," "2," "3,"]
nums fold + ',' // "1,2,3,"
nums join ',' // "1,2,3"

visibility modifiers
+	public
-	private
#	protected
~	internal

instance semantics modifiers
	(blank) immutable value
@	mutable value
*	mutable reference to mutable instance
^	mutable reference to immutable instance
&	immutable reference to mutable instance
%	immutable reference to immutable instance
?	reference/value is nullable

/*instance storage modifiers
	(blank) automatic
.	stack (scope/no-borrow)
,	heap (scope/borrow)
:	heap (deterministic/reference-counted)
;	heap (non-deterministic/garbage-collected)*/ // can we just have this inferred?

storage type modifiers
	(blank) instance
$	static

numeral system prefixes
	(blank) decimal
0x	hexedecimal
0b	binary
0o	octal

number type literal suffixes
i	signed
u	unsigned
f	floating-point
m	decimal

number type storage size suffixes (bits)
8
16
32
64

+ namespace System
	+ type Ting
		-$ strings: [] String

		+ constructor()
			strings = ""

		+ func Foo(bar & Bar) String
			let x = 5;
			let b: | = new Bar(x);
			let a = new [(64)] {1, 2, 3};

			if x == 5
				doSomething();

```