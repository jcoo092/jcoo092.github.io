Title: Deploying Then Securing the OWASP Juice Shop, Part Two of ? 
Lead: Manual deployment to AWS
Published: 12-07-2023
Tags:

- AWS
- Cloud
- Deployment
- Juice Shop
- OWASP
---
# Deploying the Juice Shop to AWS, the manual way

## Summary

A short overview/walkthrough of how I deployed the [OWASP Juice Shop](https://owasp.org/www-project-juice-shop/) web application to AWS, taking the simplistic and naïve approach.  Roughly speaking, this will use the Infrastructure-as-a-Service approach, with the deployment performed via the AWS management console in a point & click fashion.  I haven't yet figured out any details beyond that, though.

## AWS Doesn't Like Itself?

After creating an AWS root user account, I followed their [introductory instructions](https://aws.amazon.com/getting-started/guides/setup-environment/module-two/) to create a new AWS organisation and an Administrator user inside said organisation, in accordance with best practices (i.e. don't use your root user account for anything but administration of the overall organisation).

Unfortunately, when I tried to login using said organisation account, all I ever got was a rejection from AWS telling me that my credentials weren't valid.  So, I logged back into the root user account, and used their password reset flow to send a new password reset email to the org user.  I then used the link in said email to reset the password, and tried logging in again, only to be met with the exact same result.

The only thing I can think is that the instructions AWS provided are, in some way, flawed.  Or that I didn't follow them correctly, which would suggest that the instructions aren't as robust against user errors as they should be.  Plus, I'm pretty sure I followed them exactly as written.  As of the time of writing this, I still haven't resolved the issue, but I'm beginning to think that I will need to blow that org away and start all over again, which is a pain in the rear end.

Redoing the tutorial and duplicating everything _seemed_ to work.  Fingers crossed that it stays working...

### The Next Stage Isn't Better

The next step discusses [setting up the AWS CLI](https://aws.amazon.com/getting-started/guides/setup-environment/module-three/).  After telling you to install the CLI locally, it goes straight into configuration of credentials.  Which makes sense, except it dives straight into telling you what information you need, but doesn't explain anything about how to get your hands on it:

> To configure the credentials, use the command aws configure and include the credentials of the user created in the previous module of this tutorial. Add the user we included in the user group with administrator-level permissions.
> 
> When you use the aws configure command, you will be asked for:
>
> - AWS Access Key ID
> - AWS Secret Access Key
> - Default Region: Provide the Region in the following format us-east-1. For a list of Region names and codes, see this table.
> - Default Output Format: This is how the output should be displayed by default, and includes, but is not limited to: json, yaml, text. Review the documentation for all options. 

Precisely __nothing__ about where to find the relevant credentials.  Moreover, when I eventually discovered after searching through the AWS documentation that you can apparently find CLI credentials on the first page you see when you log in, I couldn't figure out how to get back to that page.  Eventually, I logged back and back in (which seemed like it almost wasn't going to work, yet again...).  That showed me a screen which said I could look up credentials for command line access, but it appeared that the only credentials I could access there were the ones for the root user account.  Given that this is new-user-onboarding documentation that AWS themselves point you to, it's rather rubbish, quite frankly.[^custserv]  I _still_ haven't figured out how to get CLI creds for the org user yet.

It is possible that I am completely misunderstanding something about how AWS works here, but if so it is completely unclear to me at this point.  Which suggests that their documentation __for beginners to AWS__ is seriously lacking. 

[^custserv]:  And I had always heard that AWS was supposed to be the cloud provider with excellent customer service, to boot.

#### AWS SSO

After eventually giving up and just using what appeared to be the creds for the admin account's CLI access, I then tried to follow the other instructions they linked to about using `aws configure sso`.  That didn't go any better, quite frankly.  While it does appear that the relevant config files have been set in my .aws folder, they just don't seem to work very well in many circumstances.  E.g. I tried to use `aws sso login`, and just got pointed to a login webpage that kept on looping through the login process, without ever updating the CLI tool.  Seriously, I went through entering my login credentials something like six times in a row, where each time after I finished it would just take me back to the start, while the CLI tool waited for a response from the API.  I just gave up in the end.

Fortunately, it seemed like I could copy-paste stuff out of the web portal with the admin creds directly into the configuration file, and that _seemed_ to work—though I also had to copy in a 'session token' field, of which no mention is made in the walkthrough.  Absolute nightmare just to get myself set up on the basics, however, especially when I FOLLOWED THE DARN AWS BEGINNER GUIDE AND IT DIDN'T FREAKING WORK.

All in all, a very poor first impression for getting myself set up independently, AWS.  Honestly, if it weren't the 800-pound gorilla of cloud services, that would have been enough to make me run screaming in the other direction and not look back.

### Dishonourable Mentions

I came across other stuff in under the getting started sections that was seemingly, at best, badly outdated.  For example:

- [Deploy a LAMP Web App on Amazon Lightsail](https://aws.amazon.com/getting-started/guides/deploy-lamp-lightsail/), which seemingly assumed that you were using Bash on a Linux machine.  It also assumed that you already had some resources install on your system, or it completely missed out at least one important step.  I couldn't figure out which, but as someone on Windows using Powershell and not greatly familiar with LAMP applications, this so-called guide was utterly useless.  Seriously, the very first command to run listed in it is `cd /opt/bitnami/apache2/htdocs rm -rf *`.  Why on earth would they assume there's even going to be a `/opt/bitnami` directory?
- 