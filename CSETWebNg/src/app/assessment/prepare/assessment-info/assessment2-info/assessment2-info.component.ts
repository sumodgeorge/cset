import { Component, OnInit, ViewChild } from '@angular/core';
import { AssessmentService } from '../../../../services/assessment.service';
import { NavigationService } from '../../../../services/navigation.service';
import { DemographicService } from '../../../../services/demographic.service';
import { ConfigService } from '../../../../services/config.service';
import { AssessmentDemographicsComponent } from '../assessment-demographics/assessment-demographics.component';
import { AssessmentContactsComponent } from '../assessment-contacts/assessment-contacts.component';
import { User } from '../../../../models/user.model';

@Component({
  selector: 'app-assessment2-info',
  templateUrl: './assessment2-info.component.html'
})
export class Assessment2InfoComponent implements OnInit {

  constructor(
    public assessSvc: AssessmentService,
    public navSvc: NavigationService,
    private demoSvc: DemographicService,
    private configSvc: ConfigService
  ) { }

  @ViewChild('contacts') contacts: AssessmentContactsComponent;
  @ViewChild('demographics') demographics: AssessmentDemographicsComponent;

  ngOnInit() {
    this.demoSvc.id = (this.assessSvc.id());
  }

  /**
   * 
   * @returns 
   */
  isDisplayed(): boolean {
    let isStandard = this.assessSvc.assessment?.UseStandard;
    let isNotAcetModel = !(this.assessSvc.usesMaturityModel('ACET'));

    let show = !this.configSvc.acetInstallation || isStandard;

    return show || isNotAcetModel;
  }

  /**
   * Ensures that the demographics are not pointing to a non-existent contact.
   * Tells the demographics component to refresh itself.
   */
  contactsUpdated() {
    console.log('assessment2-info contactsUpdated');

     // sync demographic contact list with the 'real' list
     this.demographics.contacts = [];
     this.contacts.contacts.forEach(c => {
       const cc = {
         AssessmentContactId: c.AssessmentContactId,
         AssessmentId: c.AssessmentId,
         UserId: c.UserId,
         FirstName: c.FirstName,
         LastName: c.LastName,
         Title: c.Title,
         Phone: c.Phone,
         PrimaryEmail: c.PrimaryEmail,
         ContactId: c.ContactId,
         saveEmail: c.PrimaryEmail,
         AssessmentRoleId: c.AssessmentRoleId,
         Id: c.Id,
         Invited: c.Invited
       };
       this.demographics.contacts.push(cc);
     });

     console.log(this.demographics.contacts);


    // remove facilitator if no longer on the assessment
    let exists = this.demographics.contacts.some(x => x.AssessmentContactId == this.demographics.demographicData.Facilitator);
    if (!exists) {
      this.demographics.demographicData.Facilitator = null;
    }

    // remove point of contact if no longer on the assessment
    exists = this.demographics.contacts.some(x => x.AssessmentContactId == this.demographics.demographicData.PointOfContact);
    if (!exists) {
      this.demographics.demographicData.PointOfContact = null;
    }

    const g = this.demographics.demographicData;
    this.demographics.demographicData = null;
    this.demographics.demographicData = g;

    this.demographics.updateDemographics();
  }

}
