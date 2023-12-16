---
Title: My Problem With Design Patterns
Lead: They are misunderstood and misused
date: 2023-12-16
draft: false
ShowToc: true
Tags:

  - Design patterns
  - Rants
  - Software Development
---

# I Have A Problem With Design Patterns

Just in case the title and heading didn't tip you off, I have a problem with software design patterns.  The problem isn't actually with the design patterns themselves, nor (necessarily) with the people who promote them.  My problem, at it's root, is how people treat them with an unquestioning semi-religious idolatry, and also use them as a way to declare themselves superior to others in a most idiotic fashion.

## Not Alexander's Pattern Languages

Before I go any further, I just want to point out (in case there's any confusion), I am _not_ talking about the design patterns as discussed by Christoper Alexander in his book 'A Pattern Language'.  At SPLASH 2022, there was an interesting keynote talk from a Japanese professor (whose name I have completely forgotten now...) discussing that field.  At the start of his talk, he asked who in the audience knew about design patterns.  I put my hand up, mistakenly thinking he was discussing software design patterns.  No, turns out he was discussing something older, and probably more meaningful.  Anyway, the point is that I'm vaguely aware of this other use of the term, but I am not talking about it here.  Rather, I'm talking about design patterns as usually known in software development, with names such as the Visitor, Mediator or Observer pattern.

## The Ferengi 'Bible'

The Ferengi are an alien race in the Star Trek universe.  They were first introduced really weirdly (seriously, they were like these little barbarian dudes in loincloths with electro-whips—the first series or two of Star Trek: The Next Generation was frequently bad and/or weird), but very quickly morphed into a group focused primarily on their utter obsession with gaining profit.  Star Trek: TNG largely ignored them after that, but they were explored in great detail in Star Trek: Deep Space Nine.  In DS9, they are shown to approach this obsession with wealth accumulation a religious fervour, and central to this is a holy book known as the Rules of Acquisition, which is kinda like a cross between The Art of War, Proverbs from The Bible, and any number of books published in the 1980s promoting ruthless capitalism.

The point is, the main Ferengi character in DS9, Quark, eventually somehow gets to meet one of the authors of the Rules of Acquisition (I forget exactly how it works, given that the book was supposed to have been first published a long time ago and all the authors were long dead).  The authors of the book are more-or-less seen as religious prophets, communicating the great holy truths of the universe to the Ferengi.  Which is a great surprise to this author to whom Quark speaks.  The authors merely tried to publish a book that would sell well, and named it "Rules of Acquisition" because a title like "Suggestions for Acquisition" probably wouldn't be as successful.  They never anticipated that Ferengi culture would be so strongly shaped by their book, and, if I recall correctly, the author with whom Quark talks isn't terribly impressed by how the Ferengi have chosen to throw away some of their capacity for thinking for themselves and instead slavishly following the book.

I say all this because I have the impression that something similar has gone on with design patterns, starting with the so-called 'Gang of Four' book.[^notchinagof]

[^notchinagof]:  Not to be confused with other groups labelled the Gang of Four, such as (I believe) four prominent members of the Chinese Communist Party in the late 1970s and/or early 1980s.

## The Gang of Four Book

I'm pretty sure software design patterns have existed, in some fashion, since shortly after people started programming computers.  They would have found that some approaches worked better than others, and thus continued using the better ones.  Over time, the programmers would likely have developed a sense for the overarching concepts involved, and probably communicated that to the newer programmers coming on board.  I don't know when such things first became formalised, nor when they first were referred to as "design patterns".  Certainly they were vaulted well into popularity by the book "Design Patterns: Elements of Reusable Object-Oriented Software"—commonly known as the Gang of Four book.[^gangoffourcitation]

[^gangoffourcitation]: E. Gamma, R. Helm, R. Johnson, and J. Vlissides, _Design patterns: Elements of Reusable Object-Oriented Software_. Pearson Education, 1994.

I have to admit, I have never actually got around to reading the GoF book, though I have certainly come across _many_ references to it.  So far as I can tell, it includes quite a lot of good advice for people using C++ around that time, and indeed considerable parts of it are informative for others working in an object-oriented language.  I'm sure one could find many small criticisms of it if desired,[^criticisms] but it seems like it genuinely was a pretty good programming book, and one where a lot of the advice continued to be useful years into the future (most programming books are outdated by the time they even are officially published, and ancient history after about three years).

[^criticisms]:  People certainly found things to criticise.  E.g., the suggestion that many of the patterns in the book were really just crutches to work around limitations in C++ as it was at the time, and that these patterns were wholly unneeded in other languages.  I don't know how well justified said criticisms were, but it is true that C++ back in the day was a much different beast to the modern era.

The bulk of the book focuses on 23 design patterns that the authors had discovered/figured out with experience, and kept re-using as a good way to keep the software they were developing from becoming unmanageable.  Basically, each design pattern described an architectural approach (for a wide variety of potential meanings of 'architectural'), and the sorts of scenarios where they had found it was useful.  The benefits tended to be software that was easier to develop and maintain, with relatively low costs in other regards.  My impression is essentially that was a compendium of programming/software construction techniques the authors had found useful, and intended to be the sort of book you keep around and refer to for inspiration when trying to work out how to structure a new piece of the puzzle.

If that was where we stopped, everything would be fine.  As I said before, I'm under the impression that there really is some good advice in there.

## The Problem

I said this in the introduction, but I'll repeat here:  My problem with design patterns is the quasi-religious way people approach them, as if they're the patterns are great fundamental truths of the software development universe, brought to us by the prophets the Gang of Four in their holy book.  I have genuinely see people talk as if they think that every single aspect of programming fits into one of the design patterns, and it's all simply a matter of determining which one is right for the situation at hand.  I have also seen people using knowledge of design patterns as a form of gatekeeping.  Those who are ignorant of them aren't 'real' software developers and are to be pitied.  Programmers who _are_ away of design patterns (and, usually, this specifically means those from the GoF book) but choose not to use them are heretics to be avoided or ridiculed—either way, they're clearly misguided and inferior.

Dijkstra is supposed to have said something like[^dijkstracitation]

> Please don't fall into the trap of believing that I am terribly dogmatic about [the go to statement]. I have the uncomfortable feeling that others are making a religion out of it, as if the conceptual problems of programming could be solved by a simple trick, by a simple form of coding discipline!

[^dijkstracitation]:  Taken from https://en.wikiquote.org/wiki/Edsger_W._Dijkstra#1970s, accessed on 16 December 2023.

He was, of course, referring to this famous opposition to the use of `goto` statements in typical programming, because they tended to lead to far greater problems at the large-scale than they solved at the small-scale.  Design patterns seem to me to be a similar idea, in that they're suggestions of ways to structure software to make it easier to handle in the large-scale, yet others seem to have made a wacky religion out of them.  I have a suspicion that if you take the Dijkstra quote and replace "the go to statement" with "design patterns", the Gang of Four authors might well feel similarly.  Or indeed, if they spoke to some of the zealots of the Cult of Design Patterns, they might have a similar reaction to the Rules of Acquisition author when he met Quark.

So, overall, my problem isn't with design patterns, but rather the way that people choose to approach them and abstain from thinking for themselves.  I am yet to see any set of patterns or indeed any tool that can entirely obviate the need for one's own brainpower when developing software.  The _real_ best developers know when to use the typical approach, and when and why to deviate from it.