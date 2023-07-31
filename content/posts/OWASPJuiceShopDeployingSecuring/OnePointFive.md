---
Title: Deploying Then Securing the OWASP Juice Shop, Part One-Point-Five of ?
Lead: Difficulties getting started with AWS
date: 2023-07-24
draft: false
Tags:
  
  - AWS
  - Cloud
  - Deployment
  - Documentation
  - Gripes

Series:
  - Deploying Then Securing the OWASP Juice Shop
---

# Difficulties getting started with AWS

## Summary

A short overview of some of the _many_ issues I encountered when trying to get myself up and running with AWS.

## AWS Doesn't Like Itself?

After creating an AWS root user account, I followed
their [introductory instructions](https://aws.amazon.com/getting-started/guides/setup-environment/module-two/) to create
a new AWS organisation and an Administrator user inside said organisation, in accordance with best practices (i.e. don't
use your root user account for anything but administration of the overall organisation).

Unfortunately, when I tried to login using said organisation account, all I ever got was a rejection from AWS telling me
that my credentials weren't valid. So, I logged back into the root user account, and used their password reset flow to
send a new password reset email to the org user. I then used the link in said email to reset the password, and tried
logging in again, only to be met with the exact same result.

The only thing I can think is that the instructions AWS provided are, in some way, flawed. Or that I didn't follow them
correctly, which would suggest that the instructions aren't as robust against user errors as they should be. Plus, I'm
pretty sure I followed them exactly as written. As of the time of writing this, I still haven't resolved the issue, but
I'm beginning to think that I will need to blow that org away and start all over again, which is a pain in the rear end.

Redoing the tutorial and duplicating everything _seemed_ to work. Fingers crossed that it stays working...

## The Next Stage Isn't Better

The next step
discusses [setting up the AWS CLI](https://aws.amazon.com/getting-started/guides/setup-environment/module-three/). After
telling you to install the CLI locally, it goes straight into configuration of credentials. Which makes sense, except it
dives straight into telling you what information you need, but doesn't explain anything about how to get your hands on
it:

> To configure the credentials, use the command aws configure and include the credentials of the user created in the
> previous module of this tutorial. Add the user we included in the user group with administrator-level permissions.
>
> When you use the aws configure command, you will be asked for:
>
> - AWS Access Key ID
> - AWS Secret Access Key
> - Default Region: Provide the Region in the following format us-east-1. For a list of Region names and codes, see this
    table.
> - Default Output Format: This is how the output should be displayed by default, and includes, but is not limited to:
    json, yaml, text. Review the documentation for all options.

Precisely __nothing__ about where to find the relevant credentials. Moreover, when I eventually discovered after
searching through the AWS documentation that you can apparently find CLI credentials on the first page you see when you
log in, I couldn't figure out how to get back to that page. Eventually, I logged out and back in (which seemed like it
almost wasn't going to work, yet again...). That showed me a screen which said I could look up credentials for command
line access, but it appeared that the only credentials I could access there were the ones for the root user account.
Given that this is new-user-onboarding documentation that AWS themselves point you to, it's rather rubbish, quite
frankly.[^custserv]  I _still_ haven't figured out how to get CLI creds for the org user yet.

It is possible that I am completely misunderstanding something about how AWS works here, but if so it is completely
unclear to me at this point. Which suggests that their documentation __for beginners to AWS__ is seriously lacking.

[^custserv]:  And I had always heard that AWS was supposed to be the cloud provider with excellent customer service, to
boot.

### AWS SSO

After eventually giving up and just using what appeared to be the creds for the admin account's CLI access, I then tried
to follow the other instructions they linked to about using `aws configure sso`. That didn't go any better, quite
frankly. While it does appear that the relevant config files have been set in my .aws folder, they just don't seem to
work very well in many circumstances. E.g. I tried to use `aws sso login`, and just got pointed to a login webpage that
kept on looping through the login process, without ever updating the CLI tool. Seriously, I went through entering my
login credentials something like six times in a row, where each time after I finished it would just take me back to the
start, while the CLI tool waited for a response from the API. I just gave up in the end.

Fortunately, it seemed like I could copy-paste stuff out of the web portal with the admin creds directly into the
configuration file, and that _seemed_ to work—though I also had to copy in a 'session token' field, of which no mention
is made in the walkthrough. It was an absolute nightmare just to get myself set up on the basics, however, especially
when I FOLLOWED THE DARN AWS BEGINNER GUIDE AND IT DIDN'T FREAKING WORK.

All in all, a very poor first impression for getting myself set up independently on AWS. Honestly, if it weren't the
800-pound gorilla[^armstrong] of cloud services, that would have been enough to make me run in the other direction and
not look back.

[^armstrong]:  To misuse Dr Joe Armstrong's famous quote, it definitely feels like I'm forced to deal with the whole
forest if I want to use this gorilla's banana...

## Dishonourable Mentions

I came across other stuff in under the getting started sections that was seemingly, at best, badly outdated. For
example:

- [Deploy a LAMP Web App on Amazon Lightsail](https://aws.amazon.com/getting-started/guides/deploy-lamp-lightsail/),
  which seemingly assumed that you were using Bash on a Linux machine. As someone on Windows using Powershell and not
  greatly familiar with LAMP applications, this so-called guide was rather unhelpful. Fortunately, I _could_ use WSL to
  cover this, which I guess perhaps AWS assumes people on Windows are already operating in. Plus, the Lightsail
  blueprint and bundle IDs specified in the guide were so out-of-date as to be unavailable anymore. Fortunately, in this
  instance there seemed to be pretty obvious modern versions of these. Lastly, the big blob shell script that they tell
  you to copy and paste doesn't seem to work anymore. As at the moment of writing, I'm unsure how I can observe what's
  happening on the Lightsail server to find out what went wrong. I presume that it's some sort of drift between versions
  of the LAMP Lightsail blueprint or the sample application that the tutorial tells you to clone. I don't know nearly
  enough about LAMP-stack applications to make a guess at what the issue might be.[^lightsail],[^point-and-click-lamp]

- On the [Cloud Essentials page](https://aws.amazon.com/getting-started/cloud-essentials/), one of the sample topics
  under 'Launch your first app' says "Getting Started with .NET Development on AWS with Visual Studio 2019". VS 2022 has
  already been out for well over a year at this point, and the latest (greatest?) versions of .NET are only supported in
  it. True, you probably can use 2019 for their purposes fine, but this really looks like something they need to
  revisit. I can understand some documentation getting a bit stale, but not stuff linked to on their introductory 'Cloud
  Essentials' page.

- A [newer Lightsail tutorial](https://aws.amazon.com/tutorials/deploy-webapp-lightsail/module-one/) failed on the first
  section with problems to do with building a Docker image. In their defence, this problem seems to be something arising
  from changes in Debian around installing Python packages. They
  apparently [are trying](https://www.linuxuprising.com/2023/03/next-debianubuntu-releases-will-likely.html) to
  introduce [PEP 668](https://peps.python.org/pep-0668/). It's a worthy goal on Debian's part, but unfortunately it does
  introduce a whole bunch of pain in the neck when I just want to follow a tutorial.
  Trying a user installation by appending the `--user` flag didn't help. I tried the suggested `--break-system-packages`
  flag to pass to pip on the failing `RUN` command in the Dockerfile, but even that didn't work fully—it failed on a
  subsequent step. In the end, using `--break-system-packages` and specifying `--python python3` to `pipenv` seemed to
  do the trick. Fortunately, it was largely much smoother sailing after that.
  At least, until stage 4, when they completely missed out the part where you need to use the updated container image
  that was just pushed to Lightsail to do a second deployment. It was trivial to change the previous instruction
  appropriately, but they didn't even say to do it.[^lightsail2]

[^lightsail]:  It appears that quite possibly that tutorial has been superseded
by [this one](https://aws.amazon.com/tutorials/deploy-webapp-lightsail/) (which also seems to be a much better
tutorial), but the older tutorial was the one that I was pointed to out of the gate when I had just signed up to AWS.
[^point-and-click-lamp]:  I eventually stumbled across yet another tutorial, which seems to be a point-and-click
equivalent to this first tutorial. Honestly, it seems like it's probably a much better resource given that the whole
thing is just about deploying a pre-formed LAMP-stack instance to Lightsail.
[^lightsail2]:  Actually, they did eventually get to that, but not after telling you to note that the container was
being deployed, meaning I had to infer I was already supposed to have done that. It looks like a copy-paste error. Does
nobody proofread these things?

## But You Claim You Did That Training?

I should clarify, in case anyone starts wondering:  The training for the first AWS
certification, [AWS Certified Cloud Practitioner](https://aws.amazon.com/certification/certified-cloud-practitioner/),
that I undertook via [A Cloud Guru](https://www.pluralsight.com/cloud-guru) covered all the foundational high-level
concepts that you will be tested on in the certification exam. It didn't however, walk me through getting started with
an AWS account. Rather, the exercises included used dummy accounts set up under their own AWS organisation, with login
creds provided by them. Which worked very well for the purposes of the training, to be honest. Besides, given that AWS
apparently can't keep its introductory material up to date, what hope does an outside organisation have?

## Conclusion

I found that many of the AWS introductory training materials and documentation were, at best, badly outdated. In some
instances, they rather seemed to assume that someone was already an AWS expert. It was not a very good impression at
all. One positive thing I will say about the tutorials I saw, however, was that they pretty much all ended with cleaning
up whatever resources that were stood up during the tutorial. I imagine that they probably do it so they don't need to
deal with a legion of practice instances doing nothing for nobody, but it does help prevent people suddenly getting
billed for something they have forgotten about down the line.